using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using DigiAeon.Core;
using DigiAeon.EvilApiClient.Services.Hubs;
using DigiAeon.EvilApiClient.Services.Interfaces.CustomerFile;
using DigiAeon.EvilApiClient.Services.Interfaces.EvilApi;
using Microsoft.AspNet.SignalR;

namespace DigiAeon.EvilApiClient.Services
{
    public class CustomerFileService : ServiceBase, ICustomerFileService
    {
        private readonly CustomerFileServiceSettings _settings;
        private readonly IEvilApiService _evilApiService;
        public CustomerFileService(CustomerFileServiceSettings settings, IEvilApiService evilApiService)
        {
            _settings = settings;
            _evilApiService = evilApiService;

            if (_settings.AllowedFileExtensions == null || _settings.AllowedFileExtensions.Count <= 0)
            {
                ThrowServiceDependencyException(typeof(CustomerFileServiceSettings), () => settings.AllowedFileExtensions, "is either null or has no value");
            }
            else if (string.IsNullOrWhiteSpace(_settings.FolderPath))
            {
                ThrowServiceDependencyException(typeof(CustomerFileServiceSettings), () => settings.FolderPath, "is null or blank");
            }
            else if (!Directory.Exists(_settings.FolderPath))
            {
                ThrowServiceDependencyException(typeof(CustomerFileServiceSettings), () => settings.FolderPath, "doesn't exists");
            }
        }
        public UploadResponse UploadCustomerFile(UploadRequest request)
        {
            var errorCode = UploadErrorCode.None;
            var uploadedFileName = string.Empty;

            if (request.FileStream == null)
            {
                errorCode = UploadErrorCode.FileStreamEmpty;
            }
            else if (string.IsNullOrWhiteSpace(request.FileName))
            {
                errorCode = UploadErrorCode.FileNameEmpty;
            }
            else if (_settings.AllowedFileExtensions.All(x => string.Compare(x, Path.GetExtension(request.FileName), StringComparison.InvariantCultureIgnoreCase) != 0))
            {
                errorCode = UploadErrorCode.FileTypeInvalid;
            }
            else
            {
                // I am assuming that file name is not long enought to break the hosting machine's max file path length limit (255 characters?)

                uploadedFileName = Guid.NewGuid().ToString().Replace("-", "") + "-" + request.FileName;
                FileHelper.CreateFile(Path.Combine(_settings.FolderPath, uploadedFileName), request.FileStream);
            }

            return new UploadResponse(uploadedFileName, errorCode);
        }

        public void UploadCustomersAndBroadcastResult(string userName, TextReader fileTextReader, string fileName, string broadcastForIdentityUserName)
        {
            var broadcastTaskParameters = new List<UploadAndBroadcastTaskParameters>();
            
            using (var csvReader = new CsvReader(fileTextReader, new CsvConfiguration { HasHeaderRecord = false }))
            {
                while (csvReader.Read())
                {
                    // Try to get customer
                    var customer = string.Empty;
                    var couldRetrieveCustomer = csvReader.CurrentRecord.Length >= 1 && csvReader.TryGetField(0, out customer);

                    // Try to get value
                    var value = 0;
                    var couldRetrieveValue = csvReader.CurrentRecord.Length >= 2 && csvReader.TryGetField(1, out value);

                    if (couldRetrieveCustomer && couldRetrieveValue)
                    {
                        // Upload customer and broadcast result

                        broadcastTaskParameters.Add(new UploadAndBroadcastTaskParameters
                        {
                            UserName = userName,
                            Customer = customer,
                            Value = value,
                            FileName = fileName,
                            BroadcastForIdentityUserName = broadcastForIdentityUserName
                        });
                    }
                    else
                    {
                        var response = new UploadCustomerResponse
                        {
                            Customer = customer,
                            Value = value,
                            Errors = new[] { "Couldn't parse the record" }
                        };

                        // Broadcast result for any parsing error
                        BroadcastUploadCustomerResult(fileName, response, broadcastForIdentityUserName);
                    }
                }
            }

             Task.Run(() => Parallel.ForEach(broadcastTaskParameters, param => UploadCustomerAndBroadcast(param.UserName, param.Customer, param.Value, param.FileName, param.BroadcastForIdentityUserName)));
        }

        private async void UploadCustomerAndBroadcast(string userName, string customer, int value, string fileName, string broadcastForIdentityUserName)
        {
            try
            {
                // Step 1: Send the customer data to API for upload
                var uploadResponse = await _evilApiService.UploadCustomer(userName, customer, value, fileName).ConfigureAwait(false);

                // Step 2: If it's failed no need to run again (Or this is my assumption)
                if (string.IsNullOrWhiteSpace(uploadResponse.Hash))
                {
                    BroadcastUploadCustomerResult(fileName, uploadResponse, broadcastForIdentityUserName);
                }

                // Step 2: Looks like we got the hash, now varify if customer is actually created!
                var getResponse = await _evilApiService.GetCustomer(uploadResponse.Hash).ConfigureAwait(false);

                // Step 3: Well well well, hash not found, reset the response result (don't reset hash so that it can be used for inquiry)
                if (string.IsNullOrWhiteSpace(getResponse.Hash))
                {
                    uploadResponse.Added = false;
                    uploadResponse.Errors = getResponse.Errors;
                }

                // Step 4: Broadcast the response to client
                BroadcastUploadCustomerResult(fileName, uploadResponse, broadcastForIdentityUserName);
            }
            catch (Exception ex)
            {
                var response = new UploadCustomerResponse
                {
                    Customer = customer,
                    Value = value,
                    Errors = new[] { ex.Message } // Ideally, it should be some meaningful error, or it should not on the first place!!
                };

                // Broadcast result for error
                BroadcastUploadCustomerResult(fileName, response, broadcastForIdentityUserName);
            }
        }

        private void BroadcastUploadCustomerResult(string fileName, UploadCustomerResponse response, string broadcastForIdentityUserName)
        {
            var hubContenxt = GlobalHost.ConnectionManager.GetHubContext<ServiceHub>();
            hubContenxt.Clients.User(broadcastForIdentityUserName).broadcastUploadCustomerResult(fileName, response);
        }

        private class UploadAndBroadcastTaskParameters
        {
            public string UserName { get; set; }
            public string Customer { get; set; }
            public int Value { get; set; }
            public string FileName { get; set; }
            public string BroadcastForIdentityUserName { get; set; }
        }
    }
}
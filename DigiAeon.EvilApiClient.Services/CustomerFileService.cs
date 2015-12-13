using System;
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

        public void UploadCustomersAndBroadcastResult(string userName, string fileName, string broadcastForIdentityUserName)
        {
            // TODO: Need to move OpenText code outside class and need to inject dependency for Unit Test.
            var filePath = Path.Combine(_settings.FolderPath, fileName);

            using (var reader = File.OpenText(filePath))
            {
                using (var csvReader = new CsvReader(reader, new CsvConfiguration { HasHeaderRecord = false }))
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
                            UploadCustomerAndBroadcast(userName, customer, value, fileName, broadcastForIdentityUserName);
                        }
                        else
                        {
                            var response = new UploadCustomerResponse
                            {
                                Customer = customer,
                                Value = value,
                                Errors = new[] {"Couldn't parse the record"}
                            };

                            // Broadcast result for any parsing error
                            BroadcastUploadCustomerResult(fileName, response, broadcastForIdentityUserName);
                        }
                    }
                }
            }
        }

        private void UploadCustomerAndBroadcast(string userName, string customer, int value, string fileName, string broadcastForIdentityUserName)
        {
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    // Step 1: Send the customer data to API for upload
                    var uploadResponse = await _evilApiService.UploadCustomer(userName, customer, value, fileName).ConfigureAwait(false);

                    // Step 2: If it's failed no need to run again (Or this is my assumption)
                    if (string.IsNullOrWhiteSpace(uploadResponse.Hash))
                    {
                        return uploadResponse;
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

                    return null;
                }
                catch (AggregateException ex)
                {
                    ex.Handle(e => true);

                    var response = new UploadCustomerResponse
                    {
                        Customer = customer,
                        Value = value,
                        Errors = new[] { ex.Message } // Ideally, it should be some meaningful error, or it should not on the first place!!
                    };

                    // Broadcast result for error
                    BroadcastUploadCustomerResult(fileName, response, broadcastForIdentityUserName);

                    return null;
                }
            });
        }

        private void BroadcastUploadCustomerResult(string fileName, UploadCustomerResponse response, string broadcastForIdentityUserName)
        {
            var hubContenxt = GlobalHost.ConnectionManager.GetHubContext<ServiceHub>();
            hubContenxt.Clients.User(broadcastForIdentityUserName).broadcastUploadCustomerResult(fileName, response);
        }

        /*
        public async Task<List<UploadCustomerResponse>> UploadCustomers(string userName, string fileName)
        {
            var filePath = Path.Combine(_settings.FolderPath, fileName);
            var pendingTasks = new List<Task<UploadCustomerResponse>>();
            var completeTasks = new List<UploadCustomerResponse>();

            using (var reader = File.OpenText(filePath))
            {
                using (var csvReader = new CsvReader(reader))
                {
                    while (csvReader.Read())
                    {
                        var customer = string.Empty;
                        var couldRetrieveCustomer = csvReader.CurrentRecord.Length >= 1 && csvReader.TryGetField(0, out customer);

                        var value = 0;
                        var couldRetrieveValue = csvReader.CurrentRecord.Length >= 2 && csvReader.TryGetField(1, out value);

                        if (couldRetrieveCustomer && couldRetrieveValue)
                        {
                            pendingTasks.Add(_evilApiService.UploadCustomer(userName, customer, value, fileName));
                        }
                        else
                        {
                            var response = new UploadCustomerResponse
                            {
                                Customer = customer,
                                Value = value,
                                Errors = new[] { "Couldn't parse the record" }
                            };

                            completeTasks.Add(response);
                        }
                    }

                    completeTasks.AddRange(await Task.WhenAll(pendingTasks).ConfigureAwait(false));

                    return completeTasks;
                }
            }
        }
        */
    }
}
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DigiAeon.Core;
using DigiAeon.EvilApiClient.Services.Interfaces;
using DigiAeon.EvilApiClient.Services.Interfaces.EvilApi;
using Newtonsoft.Json;

namespace DigiAeon.EvilApiClient.Services
{
    public class EvilApiService : ServiceBase, IEvilApiService
    {
        private readonly EvilApiServiceSettings _settings;
        public EvilApiService(EvilApiServiceSettings settings)
        {
            _settings = settings;

            if (string.IsNullOrWhiteSpace(_settings.ApiBaseAddress))
            {
                ThrowServiceDependencyException(typeof(CustomerFileServiceSettings), () => settings.ApiBaseAddress, "is blank");
            }
            else if (!_settings.ApiBaseAddress.IsValidUrl())
            {
                ThrowServiceDependencyException(typeof(CustomerFileServiceSettings), () => settings.ApiBaseAddress, "is not valid url");
            }
            else if (string.IsNullOrWhiteSpace(_settings.UploadUrl))
            {
                ThrowServiceDependencyException(typeof(CustomerFileServiceSettings), () => settings.UploadUrl, "is blank");
            }
            else if (!string.Format("{0}{1}", _settings.ApiBaseAddress, _settings.UploadUrl).IsValidUrl())
            {
                ThrowServiceDependencyException(typeof(CustomerFileServiceSettings), () => settings.UploadUrl, "is not valid url");
            }
            else if (string.IsNullOrWhiteSpace(_settings.UploadCustomerAction))
            {
                ThrowServiceDependencyException(typeof(CustomerFileServiceSettings), () => settings.UploadCustomerAction, "is blank");
            }
            else if (string.IsNullOrWhiteSpace(_settings.GetCustomerUrl))
            {
                ThrowServiceDependencyException(typeof(CustomerFileServiceSettings), () => settings.GetCustomerUrl, "is blank");
            }
            else if (!string.Format("{0}{1}", _settings.ApiBaseAddress, _settings.GetCustomerUrl).IsValidUrl())
            {
                ThrowServiceDependencyException(typeof(CustomerFileServiceSettings), () => settings.GetCustomerUrl, "is not valid url");
            }
        }

        public async Task<UploadCustomerResponse> UploadCustomer(string userName, string customer, int value, string fileName)
        {
            using (var client = GetHttpClient())
            {
                var request = new UploadCustomerRequest
                {
                    Property = userName,
                    Customer = customer,
                    Action = _settings.UploadCustomerAction,
                    Value = value,
                    File = fileName
                };

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(_settings.UploadUrl, content).ConfigureAwait(false);

                var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var result = JsonConvert.DeserializeObject<UploadCustomerResponse>(responseText);
                result.Customer = customer;
                result.Value = value;

                return result;
            }
        }

        public async Task<GetCustomerResponse> GetCustomer(string hash)
        {
            using (var client = GetHttpClient())
            {
                var response = await client.GetAsync(string.Format("{0}?hash={1}", _settings.GetCustomerUrl, hash)).ConfigureAwait(false);

                var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var result = JsonConvert.DeserializeObject<GetCustomerResponse>(responseText);

                return result;
            }
        }

        private HttpClient GetHttpClient()
        {
            var client = new HttpClient { BaseAddress = new Uri(_settings.ApiBaseAddress) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}
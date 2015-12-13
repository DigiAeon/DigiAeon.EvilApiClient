using System.Threading.Tasks;

namespace DigiAeon.EvilApiClient.Services.Interfaces.EvilApi
{
    public interface IEvilApiService
    {
        Task<UploadCustomerResponse> UploadCustomer(string userName, string customer, int value, string fileName);

        Task<GetCustomerResponse> GetCustomer(string hash);
    }
}
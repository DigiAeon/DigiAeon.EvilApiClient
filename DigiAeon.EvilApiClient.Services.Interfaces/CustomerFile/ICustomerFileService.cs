using System.Collections.Generic;
using System.Threading.Tasks;
using DigiAeon.EvilApiClient.Services.Interfaces.EvilApi;

namespace DigiAeon.EvilApiClient.Services.Interfaces.CustomerFile
{
    public interface ICustomerFileService
    {
        UploadResponse UploadCustomerFile(UploadRequest request);

        // Task<List<UploadCustomerResponse>> UploadCustomers(string userName, string fileName);

        void UploadCustomersAndBroadcastResult(string userName, string fileName, string broadcastForIdentityUserName);
    }
}
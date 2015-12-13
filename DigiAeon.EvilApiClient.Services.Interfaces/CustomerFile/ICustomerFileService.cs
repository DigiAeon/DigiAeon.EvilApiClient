using System.IO;

namespace DigiAeon.EvilApiClient.Services.Interfaces.CustomerFile
{
    public interface ICustomerFileService
    {
        UploadResponse UploadCustomerFile(UploadRequest request);
        
        void UploadCustomersAndBroadcastResult(string userName, TextReader fileTextReader, string fileName, string broadcastForIdentityUserName);
    }
}
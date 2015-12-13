using System.IO;

namespace DigiAeon.EvilApiClient.Services.Interfaces.CustomerFile
{
    public class UploadRequest
    {
        public Stream FileStream { get; set; }

        public string FileName { get; set; }

    }
}
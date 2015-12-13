using System.Collections.Generic;

namespace DigiAeon.EvilApiClient.Services
{
    public class CustomerFileServiceSettings
    {
        public List<string> AllowedFileExtensions { get; set; }

        public string FolderPath { get; set; }
    }
}
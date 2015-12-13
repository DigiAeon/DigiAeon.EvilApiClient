using System.Collections.Generic;

namespace DigiAeon.EvilApiClient.UI.Bootstrapper.Interfaces
{
    public interface IConfig
    {
        string Username { get; }
        string SiteHeader { get; }

        List<string> CustomerFileAllowedFileExtensions { get; }

        string CustomerFileFolderMapPath { get; }

        string EvilApiApiBaseAddress { get; }

        string EvilApiUploadUrl { get; }

        string EvilApiUploadCustomerAction { get; }

        string EvilApiGetCustomerUrl { get; }
    }
}
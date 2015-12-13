using System.Collections.Generic;
using System.Web;

namespace DigiAeon.EvilApiClient.UI.ViewModels.CustomerFile
{
    public class UploadViewModel : ViewModelBase
    {
        public UploadViewModel()
        {
            ErrorMessage = string.Empty;
        }
        public string SelectFileMessage { get; set; }
        public string InvalidFileTypeMessage { get; set; }

        public List<string> AllowedFileExtensions { get; set; }
        public string ErrorMessage { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}
using DigiAeon.EvilApiClient.Services.Interfaces.CustomerFile;
using DigiAeon.EvilApiClient.UI.Bootstrapper.Interfaces;
using DigiAeon.EvilApiClient.UI.ViewModels.CustomerFile;

namespace DigiAeon.EvilApiClient.UI.Models.CustomerFile
{
    public class UploadModel : ModelBase<UploadViewModel>
    {
        private readonly ICustomerFileService _service;
        public UploadModel(UploadViewModel viewModel, IConfig config, ICustomerFileService service) : base(viewModel, config)
        {
            _service = service;

            ViewModel.SelectFileMessage = "Please select file";
            ViewModel.InvalidFileTypeMessage = "Invalid file type";
            ViewModel.AllowedFileExtensions = Config.CustomerFileAllowedFileExtensions;
        }

        public bool UploadFile(out string uploadedFileName)
        {
            uploadedFileName = string.Empty;

            var response = _service.UploadCustomerFile(new UploadRequest
            {
                FileName = ViewModel.File != null ? ViewModel.File.FileName : string.Empty,
                FileStream = ViewModel.File != null ? ViewModel.File.InputStream : null
            });

            if (!response.IsSuccess)
            {
                switch (response.ErrorCode)
                {
                    case UploadErrorCode.FileStreamEmpty:
                    case UploadErrorCode.FileNameEmpty:
                        ViewModel.ErrorMessage = ViewModel.SelectFileMessage;
                        break;

                    case UploadErrorCode.FileTypeInvalid:
                        ViewModel.ErrorMessage = ViewModel.InvalidFileTypeMessage;
                        break;
                }
            }
            else
            {
                uploadedFileName = response.UploadedFileName;
            }

            return response.IsSuccess;
        }
    }
}
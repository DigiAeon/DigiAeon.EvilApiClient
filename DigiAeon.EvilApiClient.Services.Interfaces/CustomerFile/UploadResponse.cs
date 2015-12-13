namespace DigiAeon.EvilApiClient.Services.Interfaces.CustomerFile
{
    public class UploadResponse
    {
        public UploadResponse(string uploadedFileName, UploadErrorCode errorCode)
        {
            UploadedFileName = uploadedFileName;
            ErrorCode = errorCode;
        }
        public bool IsSuccess
        {
            get
            {
                return ErrorCode == UploadErrorCode.None;
            }
        }

        public string UploadedFileName { get; }

        public UploadErrorCode ErrorCode { get; }
    }
}
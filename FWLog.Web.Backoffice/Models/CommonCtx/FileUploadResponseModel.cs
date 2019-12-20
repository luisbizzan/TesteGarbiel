using DartDigital.Library.Web.IO;

namespace FWLog.Web.Backoffice.Models.CommonCtx
{
    public class FileUploadResponseModel
    {
        public string FileName { get; set; }

        public static FileUploadResponseModel FromFileWriterResponse(FileWriterResponse response)
        {
            return new FileUploadResponseModel
            {
                FileName = response.FileName
            };
        }
    }
}
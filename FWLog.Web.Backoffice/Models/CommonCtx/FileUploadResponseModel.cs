using DartDigital.Library.Web.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
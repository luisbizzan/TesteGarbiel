using FWLog.Web.Backoffice.EnumsAndConsts;
using FWLog.Web.Backoffice.Models.CommonCtx;
using DartDigital.Library.Helpers;
using DartDigital.Library.Web.IO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class FileController : BOBaseController
    {
        private string _cacheName = "LastCheckOldFileFromTempFolder";

        [HttpPost]
        [Route("SaveTempFile")]
        public JsonResult SaveTempFile(HttpPostedFileBase file, string configUniqueName)
        {
            DateTime? lastCheck = (DateTime?)CacheManagement.Get(_cacheName);

            if (!lastCheck.HasValue || DateTime.UtcNow > lastCheck.Value.AddDays(1))
            {
                CacheManagement.Add(_cacheName, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));
                FileHelper.DeleteOldFilesFromTempFolder();
            }

            var config = FileUploadDefinitions.GetByUniqueName(configUniqueName);

            if (config == null)
            {
                throw new HttpException(400, "Invalid config");
            }

            FileWriterResponse response = FileHelper.SaveFileToTempFolder(config, file);
            return Json(FileUploadResponseModel.FromFileWriterResponse(response));
        }

        [HttpPost]
        [Route("SaveTempFileAndCrop")]
        public JsonResult SaveTempFileAndCrop(HttpPostedFileBase file, ImageCropModel cropArea, string configUniqueName)
        {
            var config = FileUploadDefinitions.GetByUniqueName(configUniqueName);

            if (config == null || !(config is ImageUploadConfig))
            {
                throw new HttpException(400, "Invalid config");
            }

            var imageConfig = (ImageUploadConfig)config;
            ImageFormat imageFormat;

            using (Image image = imageConfig.ProcessImage(file, cropArea.Rectangle, out imageFormat))
            {
                FileWriterResponse response = FileHelper.SaveFileToTempFolder(config, file, image, imageFormat);
                return Json(FileUploadResponseModel.FromFileWriterResponse(response));
            }
        }

        [Route("Download")]
        public FileResult Download(string fileName, string configUniqueName)
        {
            try
            {
                if (fileName.NullOrEmpty() || configUniqueName.NullOrEmpty())
                {
                    throw new HttpException(400, "Bad request");
                }

                var config = FileUploadDefinitions.GetByUniqueName(configUniqueName);

                var response = FileHelper.GetFileForDownload(config, fileName);

                return File(response.File, response.ContentType);
            }
            catch (Exception ex)
            {
                Notify.Error(ex.Message);

                throw new HttpException(404, "Not found");
            }
        }
    }
}
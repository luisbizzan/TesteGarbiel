using DartDigital.Library.Web.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FWLog.Web.Backoffice.EnumsAndConsts
{
    public static class FileUploadDefinitions
    {
        public readonly static FileUploadConfig ExampleConfig = new FileUploadConfig()
        {
            UniqueName = "Example",
            FolderPrefix = "Dart",
        };

        public readonly static ImageUploadConfig ImageCropExampleConfig = new ImageUploadConfig
        {
            UniqueName = "ImageCropExample",
            FolderPrefix = "Dart",
            CropSize = new System.Drawing.Size(100, 200)
        };

        static FileUploadDefinitions()
        {
            FileUploadConfigList = new List<IFileUploadConfig>();
            FileUploadConfigList.Add(ExampleConfig);
            FileUploadConfigList.Add(ImageCropExampleConfig);
        }

        public static List<IFileUploadConfig> FileUploadConfigList { get; set; }

        public static IFileUploadConfig GetByUniqueName(string uniqueName)
        {
            var response = FileUploadConfigList.FirstOrDefault(f => f.UniqueName.Equals(uniqueName));

            return response;
        }
    }
}
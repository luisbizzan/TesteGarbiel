using FWLog.Web.Backoffice.Models.CommonCtx;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Helpers
{
    public class Notify
    {
        private TempDataDictionary _tempData;
        private readonly string _tempDataName = "Notify";

        public Notify(TempDataDictionary tempData)
        {
            _tempData = tempData;
        }

        public void Success(string text)
        {
            SetTempData(new NotifyModel
            {
                Type = NotifyType.Success,
                Text = text
            });
        }

        public void Success(string title, string text)
        {
            SetTempData(new NotifyModel
            {
                Type = NotifyType.Success,
                Title = title,
                Text = text
            });
        }

        public void Error(string text)
        {
            SetTempData(new NotifyModel
            {
                Type = NotifyType.Error,
                Text = text
            });
        }

        public void Error(string title, string text)
        {
            SetTempData(new NotifyModel
            {
                Type = NotifyType.Error,
                Title = title,
                Text = text
            });
        }

        public void Warning(string text)
        {
            SetTempData(new NotifyModel
            {
                Type = NotifyType.Warning,
                Text = text
            });
        }

        public void Warning(string title, string text)
        {
            SetTempData(new NotifyModel
            {
                Type = NotifyType.Warning,
                Title = title,
                Text = text
            });
        }

        public void Info(string text)
        {
            SetTempData(new NotifyModel
            {
                Type = NotifyType.Info,
                Text = text
            });
        }

        public void Info(string title, string text)
        {
            SetTempData(new NotifyModel
            {
                Type = NotifyType.Info,
                Title = title,
                Text = text
            });
        }

        private void SetTempData(NotifyModel model)
        {
            _tempData[_tempDataName] = model;
        }
    }

    public enum NotifyType
    {
        Success,
        Info,
        Error,
        Warning
    }
}
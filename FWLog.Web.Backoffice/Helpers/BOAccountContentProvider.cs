using FWLog.Services.Interfaces;
using DartDigital.Library.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FWLog.Web.Backoffice.Helpers
{
    public class BOAccountContentProvider : IBOAccountContentProvider
    {
        public string GetLogoUrl()
        {
            string logoPath = UrlHelper.GetAbsoluteUrl("~/Content/images/logo.png", HttpContext.Current.Request);
            return logoPath;
        }
    }
}
using DartDigital.Library.Globalization.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using FWLog.Data;
using FWLog.Services.Services;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.App_Start
{
    public class ApplicationCultureProvider : IApplicationCultureProvider
    {
        public IEnumerable<CultureInfo> GetAvailableCultures()
        {
            var uow = (UnitOfWork)DependencyResolver.Current.GetService(typeof(UnitOfWork));
            IEnumerable<ApplicationLanguage> languages = uow.ApplicationLanguageRepository.GetAllActive();
            return languages.Select(x => new CultureInfo(x.CultureName));
        }
    }
}
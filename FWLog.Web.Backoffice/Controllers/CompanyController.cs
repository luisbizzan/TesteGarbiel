using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.GeneralCtx;
using FWLog.Web.Backoffice.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class CompanyController : BOBaseController
    {
        UnitOfWork _uow;

        public CompanyController(UnitOfWork uow)
        {
            _uow = uow;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetCompanies()
        {
            ViewData["Companies"] = new SelectList(Companies, "CompanyId", "CompanyName");
            return PartialView("_ChangeCompany");
        }


        public void ChangeCompany(int companyId)
        {
            if (CompanyId == companyId)
            {
                return;
            }

            var userInfo = new BackOfficeUserInfo();           
            CookieSaveCompany(companyId);
        }
    }
}
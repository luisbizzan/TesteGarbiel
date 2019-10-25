using FWLog.Data;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
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

        public JsonResult ChangeCompany(int companyId)
        {           
            var userInfo = new BackOfficeUserInfo();
            CookieSaveCompany(companyId, userInfo.UserId.ToString());
           
            return Json(new AjaxGenericResultModel
            {
                Success = true,
                Message = Resources.CommonStrings.ChangeCompanySuccess
            }, JsonRequestBehavior.DenyGet);
        }

        public int GetCompanyId()
        {
            return CompanyId;
        }
    }
}
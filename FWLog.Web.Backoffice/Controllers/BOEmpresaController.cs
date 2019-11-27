using FWLog.Data;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using System.Linq;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class BOEmpresaController : BOBaseController
    {
        UnitOfWork _uow;

        public BOEmpresaController(UnitOfWork uow)
        {
            _uow = uow;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuscarEmpresas()
        {
            ViewData["Empresas"] = new SelectList(Empresas, "IdEmpresa", "Nome");
            return PartialView("_MudarEmpresa");
        }

        public JsonResult MudarEmpresa(int idEmpresa)
        {
            var userInfo = new BackOfficeUserInfo();
            CookieSalvarEmpresa(idEmpresa, userInfo.UserId.ToString());

            return Json(new AjaxGenericResultModel
            {
                Success = true,
                Message = Resources.CommonStrings.ChangeCompanySuccess
            }, JsonRequestBehavior.DenyGet);
        }

        public long BuscarIdEmpresa()
        {
            return IdEmpresa;
        }
    }
}
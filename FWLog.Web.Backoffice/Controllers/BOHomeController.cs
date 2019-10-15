using FWLog.Data;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class BOHomeController : BOBaseController
    {
        UnitOfWork _uow;

        public BOHomeController(UnitOfWork uow)
        {
            _uow = uow;
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}

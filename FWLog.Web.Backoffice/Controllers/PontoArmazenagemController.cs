using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.PontoArmazenagemCtx;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class PontoArmazenagemController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;

        public PontoArmazenagemController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.PontoArmazenagem.Listar)]
        public ActionResult Index()
        {
            var viewModel = new PontoArmazenagemListaViewModel();

            return View(viewModel);
        }
    }
}
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    [Route("recebimento/notas")]
    public class BORecebimentoNotaController : BOBaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
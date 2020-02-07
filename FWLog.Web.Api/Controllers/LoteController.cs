using FWLog.Data;
using FWLog.Services.Services;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class LoteController : ApiBaseController
    {
        private LoteService _loteService;
        private UnitOfWork _unitOfWork;
        public LoteController(UnitOfWork unitOfWork, LoteService loteService)
        {
            _unitOfWork = unitOfWork;
            _loteService = loteService;
        }

        [Route("api/v1/lote/conferir")]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultaNota()
        {
            await _loteService.ConferirLoteAutomatico(User.Identity.GetUserId());

            return ApiOk();
        }
    }
}
using FWLog.Data;
using FWLog.Services.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class NotaFiscalController : ApiBaseController
    {
        private NotaFiscalService _notaFiscalService;
        private UnitOfWork _unitOfWork;

        public NotaFiscalController(UnitOfWork unitOfWork, NotaFiscalService notaFiscalService)
        {
            _unitOfWork = unitOfWork;
            _notaFiscalService = notaFiscalService;
        }

        [Route("api/v1/nota")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultaNota()
        {
            await _notaFiscalService.ConsultaNotaFiscalCompra();

            return ApiOk();
        }
    }
}
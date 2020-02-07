using FWLog.Data;
using FWLog.Services.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class TransportadoraController : ApiBaseController
    {
        private TransportadoraService _transportadoraService;
        private UnitOfWork _unitOfWork;

        public TransportadoraController(UnitOfWork unitOfWork, TransportadoraService transportadoraService)
        {
            _unitOfWork = unitOfWork;
            _transportadoraService = transportadoraService;
        }

        [Route("api/v1/transportadora/integrar")]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultarTransportadora()
        {
            await _transportadoraService.ConsultarTransportadora();

            return ApiOk();
        }
    }
}
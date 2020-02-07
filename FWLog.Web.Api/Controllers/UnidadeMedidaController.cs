using FWLog.Data;
using FWLog.Services.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class UnidadeMedidaController : ApiBaseController
    {
        private UnidadeMedidaService _unidadeMedidaService;
        private UnitOfWork _unitOfWork;

        public UnidadeMedidaController(UnitOfWork unitOfWork, UnidadeMedidaService unidadeMedidaService)
        {
            _unitOfWork = unitOfWork;
            _unidadeMedidaService = unidadeMedidaService;
        }

        [Route("api/v1/unidade-medida/integrar")]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultarUnidadeMedida()
        {
            await _unidadeMedidaService.ConsultarUnidadeMedida();

            return ApiOk();
        }
    }
}
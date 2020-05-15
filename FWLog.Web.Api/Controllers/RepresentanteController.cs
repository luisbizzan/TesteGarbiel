using FWLog.Services.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    [AllowAnonymous]
    public class RepresentanteController : ApiBaseController
    {
        public RepresentanteService _representanteService;

        public RepresentanteController(RepresentanteService representanteService)
        {
            _representanteService = representanteService;
        }

        [AllowAnonymous]
        [Route("api/v1/Representante/integrar")]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultarRepresentante()
        {
            await _representanteService.ConsultarRepresentante();

            return ApiOk();
        }

        [AllowAnonymous]
        [Route("api/v1/Representante/limpar-integracao")]
        [HttpPost]
        public async Task<IHttpActionResult> LimparIntegracao()
        {
            await _representanteService.LimparIntegracao();

            return ApiOk();
        }
    }
}
using FWLog.Services.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class PedidoController : ApiBaseController
    {
        private PedidoService _pedidoService;

        public PedidoController(PedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [AllowAnonymous]
        [Route("api/v1/pedido/integrar")]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultarPedido()
        {
            await _pedidoService.ConsultarPedidoIntegracao();

            return ApiOk();
        }
    }
}
using FWLog.Data;
using FWLog.Services.Services;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class PedidoVendaController : ApiBaseController
    {
        private PedidoVendaService _pedidoVendaService;
        private UnitOfWork _unitOfWork;

        public PedidoVendaController(UnitOfWork unitOfWork, PedidoVendaService pedidoVendaService)
        {
            _unitOfWork = unitOfWork;
            _pedidoVendaService = pedidoVendaService;
        }

        [AllowAnonymous]
        [Route("api/v1/pedido-venda/integrar")]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultarPedidoVenda()
        {
            await _pedidoVendaService.ConsultaPedidoVenda();

            return ApiOk();
        }
    }
}
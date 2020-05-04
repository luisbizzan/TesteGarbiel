using DartDigital.Library.Exceptions;
using FWLog.Services.Services;
using FWLog.Web.Api.Models.ExpedicaoPedido;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class ExpedicaoController : ApiBaseController
    {
        private readonly ExpedicaoService _expedicaoService;

        public ExpedicaoController(ExpedicaoService expedicaoService)
        {
            _expedicaoService = expedicaoService;
        }

        [Route("api/v1/expedicao-pedido/iniciar-expedicao-pedido-venda")]
        [HttpPost]
        public IHttpActionResult IniciarSeparacaoPedido(IniciarExpedicaoRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                _expedicaoService.IniciarExpedicaoPedidoVenda(requisicao.IdPedidoVenda, requisicao.IdPedidoVendaVolume, IdUsuario, IdEmpresa);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }

            return ApiOk();
        }

    }
}
using DartDigital.Library.Exceptions;
using FWLog.Services.Services;
using FWLog.Web.Api.Models.SeparacaoPedido;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class SeparacaoPedidoController : ApiBaseController
    {
        private readonly SeparacaoPedidoService _separacaoPedidoService;

        public SeparacaoPedidoController(SeparacaoPedidoService separacaoPedidoService)
        {
            _separacaoPedidoService = separacaoPedidoService;
        }

        [Route("api/v1/separacao-pedido/consulta-pedido-venda")]
        [HttpGet]
        public IHttpActionResult ConsultaPedidoVendaEmSeparacao()
        {
            var idsPedidosProcessoDeSeparacao = _separacaoPedidoService.ConsultaPedidoVendaEmSeparacao(IdUsuario, IdEmpresa);

            var response = new SeparacaoPedidoResposta
            {
                PedidosProcessoDeSeparacao = idsPedidosProcessoDeSeparacao
            };

            return ApiOk(response);
        }

        //[Route("api/v1/separacao-pedido/buscar-pedido-venda/{idPedidoVenda}/{codigoDeBarras}")]
        //[HttpGet]
        //public IHttpActionResult BuscarPedidoVenda(long? idPedidoVenda, string codigoDeBarras)
        //{
        //    if (idPedidoVenda == null && string.IsNullOrEmpty(codigoDeBarras))
        //    {
        //        return ApiBadRequest("Id do pedido ou código de barras é inválido.");
        //    }

        //    BuscarPedidoVendaResposta pedidoVendaResposta;

        //    try
        //    {
        //        var response = _separacaoPedidoService.BuscarPedidoVenda(idPedidoVenda, codigoDeBarras, IdUsuario, IdEmpresa);

        //        pedidoVendaResposta = Mapper.Map<BuscarPedidoVendaResposta>(response);
        //    }
        //    catch (BusinessException ex)
        //    {
        //        return ApiBadRequest(ex.Message);
        //    }

        //    return ApiOk(pedidoVendaResposta);
        //}

        [Route("api/v1/separacao-pedido/cancelar")]
        [HttpPost]
        public async Task<IHttpActionResult> CancelarPedidoSeparacao(CancelarPedidoSeparacaoRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                await _separacaoPedidoService.CancelarPedidoSeparacao(requisicao?.IdPedidoVenda ?? 0, requisicao?.UsuarioPermissao, IdUsuario, IdEmpresa);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }

            return ApiOk();
        }
    }
}
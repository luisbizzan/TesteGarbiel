using FWLog.Services.Services;
using FWLog.Web.Api.Models.SeparacaoPedido;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    }
}
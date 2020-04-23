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

        //[Route("api/v1/separacao/separacao-pedido/buscar-pedido-venda")]
        //[HttpPost]
        //public IHttpActionResult BuscarPedidoVenda(ValidarLoteProdutoInstalacaoModelRequisicao requisicao)
        //{

        //}

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
    }
}
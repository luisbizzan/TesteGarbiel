using DartDigital.Library.Exceptions;
using FWLog.Services.Integracao;
using FWLog.Services.Services;
using System.Collections.Generic;
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

        [AllowAnonymous]
        [Route("api/v1/pedido/confirmar")]
        [HttpPost]
        public async Task<IHttpActionResult> ConfirmarSeparacao(ConfirmarSeparacaoRequest request)
        {
            var campoChave = new Dictionary<string, string> { { "NUNOTA", request.CodigoIntegracao.ToString() }, { "SEQUENCIA", request.Sequencia.ToString() } };
            await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("ItemNota", campoChave, "QTDCONFERIDA", request.Falta);
            return ApiOk();
        }
    }

    public class ConfirmarSeparacaoRequest
    {
        public int CodigoIntegracao { get; set; }
        public int Sequencia { get; set; }
        public int Falta { get; set; }
    }
}
using DartDigital.Library.Exceptions;
using FWLog.AspNet.Identity;
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

        [Route("api/v1/separacao-pedido/consultar-pedido-venda")]
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

        [Route("api/v1/separacao-pedido/consultar-pedido-venda/{referenciaPedido}")]
        [HttpGet]
        public async Task<IHttpActionResult> BuscarPedidoVenda(string referenciaPedido)
        {
            try
            {
                var permissions = await UserManager.GetPermissionsByIdEmpresaAsync(IdUsuario, IdEmpresa);

                var temPermissaoF7 = permissions.Contains(Permissions.RFSeparacao.FuncaoF7);

                var response = _separacaoPedidoService.BuscarPedidoVenda(referenciaPedido, IdEmpresa,IdUsuario, temPermissaoF7);

                return ApiOk(response);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }
        }

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

        [Route("api/v1/separacao-pedido/iniciar-separacao-pedido-venda")]
        [HttpPost]
        public async Task<IHttpActionResult> IniciarSeparacaoPedido(IniciarSeparacaoVolumeRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                await _separacaoPedidoService.IniciarSeparacaoPedidoVenda(requisicao.IdPedidoVenda, IdUsuario, IdEmpresa, requisicao.IdPedidoVendaVolume);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }

            return ApiOk();
        }

        [AllowAnonymous]
        [Route("api/v1/separacao-pedido/dividir-pedido")]
        [HttpPost]
        public async Task<IHttpActionResult> DividirPedido(long idEmpresa)
        {
            try
            {
                await _separacaoPedidoService.DividirPedido(idEmpresa);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/separacao-pedido/finalizar-volume")]
        [HttpPost]
        public async Task<IHttpActionResult> FinalizarSeparacaoVolume(FinalizarSeparacaoVolumeRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                await _separacaoPedidoService.FinalizarSeparacaoVolume(requisicao?.IdPedidoVenda ?? 0, requisicao?.IdPedidoVendaVolume ?? 0, requisicao?.IdCaixa ?? 0, IdUsuario, IdEmpresa);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/separacao-pedido/salvar-separacao-produto")]
        [HttpPost]
        public async Task<IHttpActionResult> SalvarSeparacaoProduto(SalvarSeparacaoProdutoRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                var response = await _separacaoPedidoService.SalvarSeparacaoProduto(requisicao.IdPedidoVenda, requisicao.IdProduto, requisicao.IdProdutoSeparacao, IdUsuario, IdEmpresa);

                return ApiOk(response);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }
        }
    }
}
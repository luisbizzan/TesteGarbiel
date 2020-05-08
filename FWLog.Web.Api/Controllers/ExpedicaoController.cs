using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Services.Services;
using FWLog.Web.Api.Models.Expedicao;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class ExpedicaoController : ApiBaseController
    {
        private readonly ExpedicaoService _expedicaoService;
        private readonly UnitOfWork _unitOfWork;

        public ExpedicaoController(ExpedicaoService expedicaoService, UnitOfWork unitOfWork)
        {
            _expedicaoService = expedicaoService;
            _unitOfWork = unitOfWork;
        }

        [Route("api/v1/expedicao/consultar-volume/{referenciaPedido}")]
        [HttpPost]
        public IHttpActionResult ConsultarPedidoVendaVolume(string referenciaPedido)
        {
            try
            {
                var resposta = _expedicaoService.BuscaPedidoVendaVolume(referenciaPedido, IdEmpresa);

                return ApiOk(resposta);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [Route("api/v1/expedicao/integrar-notas-fiscais")]
        [HttpPost]
        public async Task<IHttpActionResult> AtualizaNotasFiscaisPedidos()
        {
            await _expedicaoService.AtualizaNotasFiscaisPedidos();

            return ApiOk();
        }

        [Route("api/v1/expedicao/validar-transportadora-volume")]
        [HttpPost]
        public IHttpActionResult ValidaTransportadoraInstalacaoVolume(ValidaTransportadoraInstalacaoVolumeRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                _expedicaoService.ValidaTransportadoraInstalacaoVolume(requisicao?.IdPedidoVendaVolume ?? 0, requisicao?.IdTransportadora ?? 0, IdEmpresa);

            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/expedicao/validar-endereco-volume")]
        [HttpPost]
        public IHttpActionResult ValidaEnderecoInstalacaoVolume(ValidaEnderecoInstalacaoVolumeRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                _expedicaoService.ValidaEnderecoInstalacaoVolume(requisicao?.IdPedidoVendaVolume ?? 0, requisicao?.IdTransportadora ?? 0, requisicao?.IdEnderecoArmazenagem ?? 0, IdEmpresa);

            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/expedicao/salvar-instalacao-volumes")]
        [HttpPost]
        public async Task<IHttpActionResult> SalvaInstalacaoVolumes(SalvaInstalacaoVolumesRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                await _expedicaoService.SalvaInstalacaoVolumes(requisicao?.ListaVolumes, requisicao?.IdTransportadora ?? 0, requisicao?.IdEnderecoArmazenagem ?? 0, IdEmpresa, IdUsuario);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/expedicao/iniciar-expedicao-pedido-venda")]
        [HttpPost]
        public IHttpActionResult IniciarExpedicaoPedido(IniciarExpedicaoRequisicao requisicao)
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

        [Route("api/v1/expedicao/busca-enderecos-transportadora/{codigoTransportadora}")]
        [HttpGet]
        public IHttpActionResult BuscaEnderecosPorTransportadora(string codigoTransportadora)
        {
            try
            {
                var dadosVolumesInstalados = _expedicaoService.BuscaEnderecosPorTransportadora(codigoTransportadora, IdEmpresa);

                return ApiOk(dadosVolumesInstalados);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }
        }

        [Route("api/v1/expedicao/validar-volume-doca/{referenciaPedido}")]
        [HttpGet]
        public IHttpActionResult ValidarVolumeDoca(string referenciaPedido)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                var resposta = _expedicaoService.ValidarVolumeDoca(referenciaPedido, IdUsuario, IdEmpresa);
                return ApiOk(resposta);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }

        }

        [Route("api/v1/expedicao/validar-despacho-transportadora/{codigoTransportadora}")]
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult ValidarDespachoTransportadora(string codigoTransportadora)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                var resposta = _expedicaoService.ValidarDespachoTransportadora(codigoTransportadora, IdUsuario, IdEmpresa);

                return ApiOk(resposta);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }
        }

        [Route("api/v1/expedicao/finalizar-movimentacao-doca")]
        [HttpPost]
        public IHttpActionResult FinalizarMovimentacaoDoca(FinalizarMovimentacaoDocaRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                _expedicaoService.FinalizarMovimentacaoDoca(requisicao.ListaVolumes, requisicao.IdTransportadora, IdUsuario, IdEmpresa);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/expedicao/finalizar-despacho")]
        [HttpPost]
        public async Task<IHttpActionResult> FinalizarDespachoNF(FinalizarDespachoNFRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                await _expedicaoService.FinalizarDespachoNF(requisicao.IdTransportadora, requisicao.ChaveAcesso, IdUsuario, IdEmpresa);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }

            return ApiOk();
        }

        [HttpPost]
        [Route("api/v1/expedicao/romaneio/imprimir")]
        public IHttpActionResult ImprimirRomaneio(ImprimirRomaneioRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }
            else
            {
                try
                {
                    _expedicaoService.ImprimirRomaneio(requisicao?.NroRomaneio ?? 0, requisicao?.IdImpressora ?? 0, requisicao?.ImprimeSegundaVia ?? false, IdEmpresa, IdUsuario);

                    return ApiOk();
                }
                catch (BusinessException businessException)
                {
                    return ApiBadRequest(businessException.Message);
                }
            }
        }

        [Route("api/v1/expedicao/romaneio/validar-nota")]
        [HttpPost]
        public IHttpActionResult ValidarNotaFiscalRomaneio(ValidarNotaFiscalRomaneioRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                _expedicaoService.ValidarNotaFiscalRomaneio(requisicao.IdTransportadora, requisicao.ChaveAcesso);

            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/expedicao/romaneio/validar-impressora")]
        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult ValidarImpressoraRomaneio(ValidarImpressoraRomaneioRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                _expedicaoService.ValidarImpressoraRomaneio(requisicao.IdUsuario,requisicao.IdEmpresa);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }

            return ApiOk();
        }
    }
}
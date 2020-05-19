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
        public async Task<IHttpActionResult> AtualizarNotasFiscaisPedidos()
        {
            await _expedicaoService.AtualizaNotasFiscaisPedidos();

            return ApiOk();
        }

        [Route("api/v1/expedicao/validar-transportadora-volume")]
        [HttpPost]
        public IHttpActionResult ValidarTransportadoraInstalacaoVolume(ValidaTransportadoraInstalacaoVolumeRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                _expedicaoService.ValidarTransportadoraInstalacaoVolume(requisicao?.IdPedidoVendaVolume ?? 0, requisicao?.IdTransportadora ?? 0, IdEmpresa);

            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/expedicao/validar-endereco-volume")]
        [HttpPost]
        public IHttpActionResult ValidarEnderecoInstalacaoVolume(ValidaEnderecoInstalacaoVolumeRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                _expedicaoService.ValidarEnderecoInstalacaoVolume(requisicao?.IdPedidoVendaVolume ?? 0, requisicao?.IdTransportadora ?? 0, requisicao?.IdEnderecoArmazenagem ?? 0, IdEmpresa);

            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/expedicao/salvar-instalacao-volumes")]
        [HttpPost]
        public async Task<IHttpActionResult> SalvarInstalacaoVolumes(SalvaInstalacaoVolumesRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                await _expedicaoService.SalvarInstalacaoVolumes(requisicao?.ListaVolumes, requisicao?.IdTransportadora ?? 0, requisicao?.IdEnderecoArmazenagem ?? 0, IdEmpresa, IdUsuario);
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

        [Route("api/v1/expedicao/busca-enderecos-transportadora/{idTransportadora}")]
        [HttpGet]
        public IHttpActionResult BuscaEnderecosPorTransportadora(long idTransportadora)
        {
            try
            {
                var dadosVolumesInstalados = _expedicaoService.BuscaEnderecosPorTransportadora(idTransportadora, IdEmpresa);

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
                var resposta = _expedicaoService.ValidarVolumeDoca(referenciaPedido, IdEmpresa);
                return ApiOk(resposta);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }

        }

        [Route("api/v1/expedicao/validar-despacho-transportadora/{idTransportadora}")]
        [HttpGet]
        public IHttpActionResult ValidarDespachoTransportadora(long idTransportadora)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                var resposta = _expedicaoService.ValidarDespachoTransportadora(idTransportadora, IdEmpresa);

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
                    _expedicaoService.ImprimirRomaneio(requisicao?.IdRomaneio ?? 0, requisicao?.IdImpressora ?? 0, requisicao?.ImprimeSegundaVia ?? false, IdEmpresa, IdUsuario);

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
        [HttpPost]
        public IHttpActionResult ValidarImpressoraRomaneio(ValidarImpressoraRomaneioRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                _expedicaoService.ValidarImpressoraRomaneio(requisicao.IdUsuario, requisicao.IdEmpresa);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/expedicao/romaneio/validar-transportadora/{idTransportadora}")]
        [HttpPost]
        public IHttpActionResult ValidarRomaneioTransportadora(long idTransportadora)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                var resposta = _expedicaoService.ValidarRomaneioTransportadora(idTransportadora, IdEmpresa);

                return ApiOk(resposta);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }
        }

        [Route("api/v1/expedicao/romaneio/{nroRomaneio}")]
        [HttpGet]
        public IHttpActionResult BuscarRomaneio(int nroRomaneio)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                var resposta = _expedicaoService.BuscarRomaneio(nroRomaneio, IdEmpresa);

                return ApiOk(resposta);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }
        }

        [Route("api/v1/expedicao/romaneio/finalizar-nota")]
        [HttpPost]
        public async Task<IHttpActionResult> FinalizarRomaneioNF(FinalizarRomaneioNFRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                var response = await _expedicaoService.FinalizarRomaneioNF(requisicao.IdTransportadora, requisicao.ChaveAcesso, IdUsuario, IdEmpresa);

                return ApiOk(response);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }
        }

        [Route("api/v1/expedicao/romaneio/reimprimir")]
        [HttpPost]
        public IHttpActionResult ReimprimirRomaneio(RomaneioReimprimirRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                _expedicaoService.ReimprimirRomaneio(requisicao.IdRomaneio, requisicao.IdImpressora, IdEmpresa, IdUsuario);

                return ApiOk();
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }
        }

        [Route("api/v1/expedicao/doca/validar-transportadora/{idOuCodtransportadora}")]
        [HttpGet]
        public IHttpActionResult ValidarRemoverDocaTransportadora(string idOuCodtransportadora)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                var resposta = _expedicaoService.ValidarRemoverDocaTransportadora(idOuCodtransportadora, IdEmpresa);

                return ApiOk(resposta);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }
        }

        [Route("api/v1/expedicao/doca/remover")]
        [HttpPost]
        public IHttpActionResult RemoverVolumeDoca(RemoverVolumeDocaRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                _expedicaoService.RemoverVolumeDoca(requisicao.ReferenciaPedido, IdUsuario, IdEmpresa);

                return ApiOk();
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }

        }
    }
}
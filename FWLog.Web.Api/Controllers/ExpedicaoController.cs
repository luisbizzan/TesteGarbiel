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

        [Route("api/v1/expedicao/busca-pedido-volume/{referenciaPedido}")]
        [HttpPost]
        public IHttpActionResult BuscaPedidoVendaVolume(string referenciaPedido)
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
                _expedicaoService.ValidaEnderecoInstalacaoVolume(requisicao?.IdPedidoVendaVolume ?? 0, requisicao?.IdEnderecoArmazenagem ?? 0, IdEmpresa);

            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/expedicao/salva-instalacao-volumes")]
        [HttpPost]
        public async Task<IHttpActionResult> SalvaInstalacaoVolumes(SalvaInstalacaoVolumesRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                await _expedicaoService.SalvaInstalacaoVolumes(requisicao?.ListaVolumes, requisicao?.IdEnderecoArmazenagem ?? 0, IdEmpresa, IdUsuario);

            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }

            return ApiOk();
        }
    }
}
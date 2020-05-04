using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Services.Services;
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
    }
}
using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Services.Model.Transportadora;
using FWLog.Services.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class TransportadoraController : ApiBaseController
    {
        private TransportadoraService _transportadoraService;
        private UnitOfWork _unitOfWork;

        public TransportadoraController(UnitOfWork unitOfWork, TransportadoraService transportadoraService)
        {
            _unitOfWork = unitOfWork;
            _transportadoraService = transportadoraService;
        }

        [AllowAnonymous]
        [Route("api/v1/transportadora/integrar")]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultarTransportadora()
        {
            await _transportadoraService.ConsultarTransportadora();

            return ApiOk();
        }

        [AllowAnonymous]
        [Route("api/v1/transportadora/limpar-integracao")]
        [HttpPost]
        public async Task<IHttpActionResult> LimparIntegracao()
        {
            await _transportadoraService.LimparIntegracao();

            return ApiOk();
        }

        [Route("api/v1/transportadora/pesquisar/{codigoOuIdTransportadora}")]
        [HttpGet]
        public IHttpActionResult ConsultarTransportadora(string codigoOuIdTransportadora)
        {
            var transportadora = _transportadoraService.ConsultarTransportadora(codigoOuIdTransportadora);

            if (transportadora == null)
            {
                return ApiNotFound("Nenhuma transportadora foi encontrada.");
            }

            return ApiOk(transportadora);
        }

        [Route("api/v1/transportadora/busca-enderecos-transportadora")]
        [HttpPost]
        public IHttpActionResult BuscaEnderecosPorTransportadora(EnderecosPorTransportadoraRequisicao request)
        {
            try
            {
                var dadosVolumesInstalados = _transportadoraService.BuscaEnderecosPorTransportadora(request.IdTransportadora, IdEmpresa);

                return ApiOk(dadosVolumesInstalados);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }
        }
    }
}
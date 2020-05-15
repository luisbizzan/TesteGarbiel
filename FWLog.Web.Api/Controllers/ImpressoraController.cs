using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Services;
using FWLog.Web.Api.Models.Impressora;
using System.Linq;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class ImpressoraController : ApiBaseController
    {
        private readonly ImpressoraService _impressoraService;
        private readonly UnitOfWork _unitOfWork;

        public ImpressoraController(UnitOfWork unitOfWork, ImpressoraService impressoraService)
        {
            _unitOfWork = unitOfWork;
            _impressoraService = impressoraService;
        }

        [HttpGet]
        [Route("api/v1/impressoras/{tipoImpressao}")]
        public IHttpActionResult Impressoras([FromUri]ImpressaoItemEnum tipoImpressao)
        {
            long idPerfilImpressora = _unitOfWork.UsuarioEmpresaRepository.Tabela().FirstOrDefault(x => x.IdEmpresa == IdEmpresa && x.UserId == IdUsuario)?.IdPerfilImpressoraPadrao ?? 0;

            tipoImpressao = ImpressaoItemEnum.RelatorioA4;

            ImpressorasResposta impressoras = new ImpressorasResposta { Lista = _unitOfWork.BOPrinterRepository.ObterPorPerfil(idPerfilImpressora, tipoImpressao).Select(x => new ImpressoraResposta { Id = x.Id, Name = x.Name }).ToList() };

            return ApiOk(impressoras);
        }
    }
}
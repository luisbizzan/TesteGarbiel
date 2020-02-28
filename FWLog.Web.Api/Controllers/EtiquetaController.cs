using FWLog.Data;
using FWLog.Services.Services;
using FWLog.Web.Api.Models.Etiqueta;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class EtiquetaController : ApiBaseController
    {
        private readonly EtiquetaService _etiquetaService;
        private readonly UnitOfWork _unitOfWork;

        public EtiquetaController(UnitOfWork unitOfWork, EtiquetaService etiquetaService)
        {
            _unitOfWork = unitOfWork;
            _etiquetaService = etiquetaService;
        }

        [HttpPost]
        [Route("api/v1/etiqueta/endereco")]
        public async Task<IHttpActionResult> ImprimirEtiquetaEndereco(ImprimirEtiquetaEnderecoRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            //chamar service

            return ApiOk();
        }
    }
}
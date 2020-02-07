using FWLog.Data;
using FWLog.Services.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class EmpresaController : ApiBaseController
    {
        private EmpresaService _empresaService;
        private UnitOfWork _unitOfWork;

        public EmpresaController(UnitOfWork unitOfWork, EmpresaService empresaService)
        {
            _unitOfWork = unitOfWork;
            _empresaService = empresaService;
        }

        [Route("api/v1/empresa/integrar")]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultarEmpresa()
        {
            await _empresaService.ConsultarEmpresaIntegracao();

            return ApiOk();
        }
    }
}
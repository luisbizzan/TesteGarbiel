using FWLog.Data;
using FWLog.Services.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class FreteTipoController : ApiBaseController
    {
        private FreteTipoService _freteTipoService;
        private UnitOfWork _unitOfWork;

        public FreteTipoController(UnitOfWork unitOfWork, FreteTipoService freteTipoService)
        {
            _unitOfWork = unitOfWork;
            _freteTipoService = freteTipoService;
        }

        [Route("api/v1/fretetipo/integrar")]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultarFreteTipo()
        {
            await _freteTipoService.ConsultarFreteTipo();

            return ApiOk();
        }
    }
}
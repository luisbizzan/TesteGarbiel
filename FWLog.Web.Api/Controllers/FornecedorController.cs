using FWLog.Services.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class FornecedorController : ApiBaseController
    {
        private FornecedorService _fornecedorService;

        public FornecedorController(FornecedorService fornecedorService)
        {
            _fornecedorService = fornecedorService;
        }

        [AllowAnonymous]
        [Route("api/v1/fornecedor/integrar")]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultarFornecedor(bool somenteNovos = true)
        {
            await _fornecedorService.ConsultarFornecedor(somenteNovos);

            return ApiOk();
        }
    }
}
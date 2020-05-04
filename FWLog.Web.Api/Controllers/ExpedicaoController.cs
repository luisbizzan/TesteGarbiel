using FWLog.Services.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class ExpedicaoController : ApiBaseController
    {
        private readonly ExpedicaoService _expedicaoService;

        public ExpedicaoController(ExpedicaoService expedicaoService)
        {
            _expedicaoService = expedicaoService;
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
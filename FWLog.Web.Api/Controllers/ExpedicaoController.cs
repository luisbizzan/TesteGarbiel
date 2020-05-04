using FWLog.Services.Services;

namespace FWLog.Web.Api.Controllers
{
    public class ExpedicaoController : ApiBaseController
    {
        private readonly ExpedicaoService _expedicaoService;

        public ExpedicaoController(ExpedicaoService expedicaoService)
        {
            _expedicaoService = expedicaoService;
        }
    }
}
using FWLog.Services.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    [AllowAnonymous]
    public class ClienteController : ApiBaseController
    {
        public ClienteService _clienteService;

        public ClienteController(ClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [Route("api/v1/Cliente/integrar")]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultarCliente()
        {
            await _clienteService.ConsultarCliente();

            return ApiOk();
        }
    }
}
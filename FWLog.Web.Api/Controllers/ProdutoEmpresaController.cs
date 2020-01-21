using FWLog.Data;
using FWLog.Services.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class ProdutoEmpresaController : ApiBaseController
    {
        private ProdutoEmpresaService _produtoEmpresaService;
        private UnitOfWork _unitOfWork;

        public ProdutoEmpresaController(UnitOfWork unitOfWork, ProdutoEmpresaService produtoEmpresaService)
        {
            _unitOfWork = unitOfWork;
            _produtoEmpresaService = produtoEmpresaService;
        }

        [Route("api/v1/produtoempresa")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultarProdutoEmpresa()
        {
            await _produtoEmpresaService.ConsultarProdutoEmpresaIntegracao();

            return ApiOk();
        }
    }
}
using FWLog.Data;
using FWLog.Services.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class ProdutoController : ApiBaseController
    {
        private ProdutoService _produtoService;
        private UnitOfWork _unitOfWork;

        public ProdutoController(UnitOfWork unitOfWork, ProdutoService produtoService)
        {
            _unitOfWork = unitOfWork;
            _produtoService = produtoService;
        }

        [Route("api/v1/produto")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultarProduto()
        {
            await _produtoService.ConsultarProdutoIntegracao();

            return ApiOk();
        }

        [Route("api/v1/produtoempresa")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultarProdutoPrazoEntrega()
        {
            await _produtoService.ConsultarProdutoPrazoEntrega();

            return ApiOk();
        }
    }
}
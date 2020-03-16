using FWLog.Data;
using FWLog.Services.Services;
using FWLog.Web.Api.Models.Produto;
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

        [Route("api/v1/produto/integrar")]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultarProduto()
        {
            await _produtoService.ConsultarProdutoIntegracao();

            return ApiOk();
        }

        [Route("api/v1/produto/limpar-integracao")]
        [HttpPost]
        public async Task<IHttpActionResult> LimparIntegracao()
        {
            await _produtoService.LimparIntegracao();

            return ApiOk();
        }

        [Route("api/v1/produto-prazo-entrega/limpar-integracao")]
        [HttpPost]
        public async Task<IHttpActionResult> LimparIntegracaoPrazoEntrega()
        {
            await _produtoService.LimparIntegracaoMediaVenda();

            return ApiOk();
        }

        [Route("api/v1/produto-prazo-entrega/integrar")]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultarProdutoPrazoEntrega()
        {
            await _produtoService.ConsultarProdutoPrazoEntrega();

            return ApiOk();
        }

        [Route("api/v1/produto-media-venda/limpar-integracao")]
        [HttpPost]
        public async Task<IHttpActionResult> LimparIntegracaoMediaVenda()
        {
            await _produtoService.LimparIntegracaoMediaVenda();

            return ApiOk();
        }

        [Route("api/v1/produto-media-venda/integrar")]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultarMeviaVenda()
        {
            await _produtoService.ConsultarMediaVenda();

            return ApiOk();
        }

        [Route("api/v1/produto-quantidade-reservada/integrar")]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultarQuantidadeReservada(ProdutoReservadoModelRequest request)
        {
            var quantidadeReservada = await _produtoService.ConsultarQuantidadeReservada(request.IdProduto, request.IdEmpresa);

            return ApiOk(quantidadeReservada);
        }
    }
}
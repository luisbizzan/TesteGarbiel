using AutoMapper;
using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
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

        [AllowAnonymous]
        [Route("api/v1/produto/integrar")]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultarProduto(bool somenteNovos = true)
        {
            await _produtoService.ConsultarProdutoIntegracao(somenteNovos);

            return ApiOk();
        }

        [AllowAnonymous]
        [Route("api/v1/produto/limpar-integracao")]
        [HttpPost]
        public async Task<IHttpActionResult> LimparIntegracao()
        {
            await _produtoService.LimparIntegracao();

            return ApiOk();
        }

        [AllowAnonymous]
        [Route("api/v1/produto-prazo-entrega/limpar-integracao")]
        [HttpPost]
        public async Task<IHttpActionResult> LimparIntegracaoPrazoEntrega()
        {
            await _produtoService.LimparIntegracaoMediaVenda();

            return ApiOk();
        }

        [AllowAnonymous]
        [Route("api/v1/produto-prazo-entrega/integrar")]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultarProdutoPrazoEntrega()
        {
            await _produtoService.ConsultarProdutoPrazoEntrega();

            return ApiOk();
        }

        [AllowAnonymous]
        [Route("api/v1/produto-media-venda/limpar-integracao")]
        [HttpPost]
        public async Task<IHttpActionResult> LimparIntegracaoMediaVenda()
        {
            await _produtoService.LimparIntegracaoMediaVenda();

            return ApiOk();
        }

        [AllowAnonymous]
        [Route("api/v1/produto-media-venda/integrar")]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultarMeviaVenda()
        {
            await _produtoService.ConsultarMediaVenda();

            return ApiOk();
        }

        [AllowAnonymous]
        [Route("api/v1/produto-quantidade-reservada/integrar")]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultarQuantidadeReservada(ProdutoReservadoModelRequest request)
        {
            var quantidadeReservada = await _produtoService.ConsultarQuantidadeReservada(request.IdProduto, request.IdEmpresa);

            return ApiOk(quantidadeReservada);
        }

        [HttpGet]
        [Route("api/v1/produto/pesquisar/{codref}")]
        public async Task<IHttpActionResult> Pesquisar(string codref)
        {
            if (string.IsNullOrEmpty(codref))
            {
                return ApiBadRequest("Informe o código de barras ou referência do produto.");
            }

            Produto produto = _unitOfWork.ProdutoRepository.PesquisarPorCodigoBarras(codref);

            if (produto == null)
            {
                produto = _unitOfWork.ProdutoRepository.PesquisarPorReferencia(codref);
            }

            if (produto == null)
            {
                produto = _unitOfWork.ProdutoRepository.PesquisarPorCodigoBarras2(codref);
            }

            if (produto == null)
            {
                return ApiNotFound("Nenhum produto foi encontrado.");
            }

            var produtoResposta = Mapper.Map<PesquisarPorCodigoBarrasReferenciaResposta>(produto);

            LoadCodigoEnderecoPicking(produto, produtoResposta);

            return ApiOk(produtoResposta);
        }

        private void LoadCodigoEnderecoPicking(Produto produto, PesquisarPorCodigoBarrasReferenciaResposta produtoResposta)
        {
            var produtoEstoque = _unitOfWork.ProdutoEstoqueRepository.ObterPorProdutoEmpresa(produto.IdProduto, IdEmpresa);

            if (produtoEstoque != null && produtoEstoque.EnderecoArmazenagem != null)
            {
                produtoResposta.CodigoEnderecoPicking = produtoEstoque.EnderecoArmazenagem.Codigo;
            }
        }

        [HttpGet]
        [Route("api/v1/produto/{id}")]
        public async Task<IHttpActionResult> Pesquisar(long id)
        {
            Produto produto = _unitOfWork.ProdutoRepository.GetById(id);

            if (produto == null)
            {
                return ApiNotFound("Nenhum produto foi encontrado.");
            }

            var produtoResposta = Mapper.Map<PesquisarPorCodigoBarrasReferenciaResposta>(produto);

            LoadCodigoEnderecoPicking(produto, produtoResposta);

            return ApiOk(produtoResposta);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/v1/produto/estoque/{id}")]
        public async Task<IHttpActionResult> ConsultarProdutoEstoque(long id)
        {
            if (id <= 0)
            {
                return ApiBadRequest("Informe código do produto.");
            }

            var resposta = await _produtoService.ConsultarProdutoEstoque(id, IdEmpresa);

            return ApiOk(resposta);
        }

        [Route("api/v1/produto/entradas/{idProduto}")]
        [HttpGet]
        public IHttpActionResult ConsultarEntradasProduto(long idProduto)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                var response = _produtoService.ConsultarEntradasProduto(idProduto, IdEmpresa);

                return ApiOk(response);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }
        }
    }
}
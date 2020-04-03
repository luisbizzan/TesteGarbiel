using FWLog.Data;
using FWLog.Services.Services;
using FWLog.Web.Api.Models.AtividadeEstoque;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class AtividadeEstoqueController : ApiBaseController
    {
        private AtividadeEstoqueService _atividadeEstoqueService;
        public AtividadeEstoqueController(AtividadeEstoqueService atividadeEstoqueService)
        {
            _atividadeEstoqueService = atividadeEstoqueService;
        }

        [Route("api/v1/atividade-estoque/pesquisar")]
        [HttpGet]
        public IHttpActionResult PesquisarAtividade()       
        {
            var resposta = new AtividadesEstoqueResposta
            {
                Lista = _atividadeEstoqueService.PesquisarAtividade(IdEmpresa)
            };

            return ApiOk(resposta);
        }
    }
}
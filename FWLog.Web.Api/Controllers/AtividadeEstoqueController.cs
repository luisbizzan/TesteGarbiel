using FWLog.Data;
using FWLog.Services.Services;
using System;
using FWLog.Web.Api.Models.AtividadeEstoque;
using System.Web.Http;
using FWLog.Services.Model.AtividadeEstoque;

namespace FWLog.Web.Api.Controllers
{
    [AllowAnonymous]
    public class AtividadeEstoqueController : ApiBaseController
    {
        private readonly AtividadeEstoqueService _atividadeEstoqueService;
        private readonly UnitOfWork _unitOfWork;

        public AtividadeEstoqueController(UnitOfWork unitOfWork, AtividadeEstoqueService atividadeEstoqueService)
        {
            _atividadeEstoqueService = atividadeEstoqueService;
			_unitOfWork = unitOfWork;
        }

        [Route("api/v1/atividade-estoque/cadastrar/abastecer-picking")]
        [HttpPost]
        public IHttpActionResult CadastrarAtividadeAbastecerPicking(long idEmpresa)
        {
            try
            {
                _atividadeEstoqueService.ValidarCadastroAtividade(idEmpresa);

                _atividadeEstoqueService.CadastrarAtividadeAbastecerPicking(idEmpresa);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }
            catch
            {
                throw;
            }

            return ApiOk();
        }

        [Route("api/v1/atividade-estoque/cadastrar/conferencia-endereco")]
        [HttpPost]
        public IHttpActionResult CadastrarAtividadeConferenciaEndereco(long idEmpresa)
        {
            try
            {
                _atividadeEstoqueService.ValidarCadastroAtividade(idEmpresa);

                _atividadeEstoqueService.CadastrarAtividadeConferenciaEndereco(idEmpresa);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }
            catch
            {
                throw;
            }

            return ApiOk();
        }

        [Route("api/v1/atividade-estoque/cadastrar/conferencia-399-400")]
        [HttpPost]
        public IHttpActionResult CadastrarAtividadeConferencia399_400(long idEmpresa)
        {
            try
            {
                _atividadeEstoqueService.ValidarCadastroAtividade(idEmpresa);

                _atividadeEstoqueService.CadastrarAtividadeConferecia399_400(idEmpresa);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }
            catch
            {
                throw;
            }

            return ApiOk();
		}

        [Route("api/v1/atividade-estoque/atualizar")]
        [HttpPost]
        public IHttpActionResult AtualizarAtividade(AtividadeEstoqueRequisicao atividadeEstoqueRequisicao)
        {
            try
            {
                _atividadeEstoqueService.ValidarAtualizacaoAtividade(atividadeEstoqueRequisicao);

                _atividadeEstoqueService.AtualizarAtividade(atividadeEstoqueRequisicao);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }
            catch
            {
                throw;
            }

            return ApiOk();
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
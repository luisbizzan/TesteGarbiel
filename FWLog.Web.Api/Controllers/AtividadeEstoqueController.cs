using FWLog.Data;
using FWLog.Services.Services;
using System;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    [AllowAnonymous]
    public class AtividadeEstoqueController : ApiBaseController
    {
        private readonly AtividadeEstoqueService _atividadeEstoqueService;
        private readonly UnitOfWork _unitOfWork;

        public AtividadeEstoqueController(UnitOfWork unitOfWork, AtividadeEstoqueService atividadeEstoqueService)
        {
            _unitOfWork = unitOfWork;
            _atividadeEstoqueService = atividadeEstoqueService;
        }

        [Route("api/v1/atividade/cadastrar/abastecer-picking")]
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

        [Route("api/v1/atividade/cadastrar/conferencia-endereco")]
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

        [Route("api/v1/atividade/cadastrar/conferencia-399-400")]
        [HttpPost]
        public IHttpActionResult CadastrarAtividadeConferencia399_400(long idEmpresa)
        {
            try
            {
                _atividadeEstoqueService.ValidarCadastroAtividade(idEmpresa);

                _atividadeEstoqueService.CadastrarrAtividadeConferecia399_400(idEmpresa);
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
    }
}
using DartDigital.Library.Exceptions;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Model.AtividadeEstoque;
using FWLog.Services.Services;
using FWLog.Web.Api.Models.AtividadeEstoque;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class AtividadeEstoqueController : ApiBaseController
    {
        private readonly AtividadeEstoqueService _atividadeEstoqueService;
        private readonly UnitOfWork _unitOfWork;

        public AtividadeEstoqueController(UnitOfWork unitOfWork, AtividadeEstoqueService atividadeEstoqueService)
        {
            _atividadeEstoqueService = atividadeEstoqueService;
            _unitOfWork = unitOfWork;
        }

        [AllowAnonymous]
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

            return ApiOk();
        }

        [AllowAnonymous]
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

            return ApiOk();
        }

        [AllowAnonymous]
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

            return ApiOk();
        }

        [AllowAnonymous]
        [Route("api/v1/atividade-estoque/atualizar-abastecer-picking")]
        [HttpPost]
        public IHttpActionResult AtualizarAtividadeAbastecerPicking(AtividadeEstoqueRequisicao atividadeEstoqueRequisicao)
        {
            try
            {
                _atividadeEstoqueService.ValidarAtualizacaoAtividade(atividadeEstoqueRequisicao, IdUsuario);

                var atividadeFinalizada = _atividadeEstoqueService.AtualizarAtividadeAbastecerPicking(atividadeEstoqueRequisicao, IdEmpresa, IdUsuario);

                var resposta = new AtualizarAtividadeEstoqueAbastecerPickingResposta()
                {
                    AtividadeFinalizada = atividadeFinalizada
                };

                return ApiOk(resposta);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [Route("api/v1/atividade-estoque/pesquisar/{idAtividadeEstoqueTipo}")]
        [HttpGet]
        public async Task<IHttpActionResult> PesquisarAtividade(int idAtividadeEstoqueTipo)
        {
            try
            {
                var empresaUsuario = _unitOfWork.UsuarioEmpresaRepository.Obter(IdEmpresa, IdUsuario);

                if (empresaUsuario == null)
                {
                    throw new BusinessException("O usuário não tem configuração para esta empresa.");
                }

                var tiposAtividade = new List<int>();

                if (idAtividadeEstoqueTipo == 0)
                {
                    var permissoes = await UserManager.GetPermissionsByIdEmpresaAsync(IdUsuario, IdEmpresa);

                    if (permissoes == null)
                    {
                        throw new BusinessException("Não existem permissões configuradas para o usuário.");
                    }

                    if (permissoes.Any(w => w == Permissions.RFArmazenagem.AtividadeAbastecerPicking))
                    {
                        tiposAtividade.Add(AtividadeEstoqueTipoEnum.AbastecerPicking.GetHashCode());
                    }

                    if (permissoes.Any(w => w == Permissions.RFArmazenagem.AtividadeConferenciaEndereco))
                    {
                        tiposAtividade.Add(AtividadeEstoqueTipoEnum.ConferenciaEndereco.GetHashCode());
                    }

                    if (permissoes.Any(w => w == Permissions.RFArmazenagem.AtividadeConferencia399_400))
                    {
                        tiposAtividade.Add(AtividadeEstoqueTipoEnum.ConferenciaProdutoForaLinha.GetHashCode());
                    }
                }
                else
                {
                    tiposAtividade.Add(idAtividadeEstoqueTipo);
                }

                var resposta = new AtividadesEstoqueResposta
                {
                    CorredorInicio = empresaUsuario.CorredorEstoqueInicio,
                    CorredorFim = empresaUsuario.CorredorEstoqueFim,
                    Lista = _atividadeEstoqueService.PesquisarAtividade(IdEmpresa, IdUsuario, tiposAtividade)
                };

                return ApiOk(resposta);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }
        }

        [Authorize]
        [Route("api/v1/atividade-estoque/conferencia-399-400/validar-produto")]
        [HttpPost]
        public IHttpActionResult ValidarProdutoConferenciaProdutoForaLinha(ValidarProdutoConferenciaProdutoForaLinhaRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                _atividadeEstoqueService.ValidarProdutoConferenciaProdutoForaLinha(requisicao?.Corredor ?? 0, requisicao?.IdProduto ?? 0, IdEmpresa);

                return ApiOk();
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }
        }

        [Authorize]
        [Route("api/v1/atividade-estoque/conferencia-399-400/validar-endereco")]
        [HttpPost]
        public IHttpActionResult ValidarEnderecoConferenciaProdutoForaLinha(ValidarEnderecoConferenciaProdutoForaLinhaRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                _atividadeEstoqueService.ValidarEnderecoConferenciaProdutoForaLinha(requisicao?.Corredor ?? 0,
                                                                                    requisicao?.IdEnderecoArmazenagem ?? 0,
                                                                                    requisicao?.IdProduto ?? 0,
                                                                                    IdEmpresa);

                return ApiOk();
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }
        }

        [Authorize]
        [Route("api/v1/atividade-estoque/conferencia-399-400/validar-quantidade")]
        [HttpPost]
        public IHttpActionResult ValidarQuantidadeConferenciaProdutoForaLinha(ValidarQuantidadeConferenciaProdutoForaLinhaRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                _atividadeEstoqueService.ValidarQuantidadeConferenciaProdutoForaLinha(requisicao?.Corredor ?? 0,
                                                                                    requisicao?.IdEnderecoArmazenagem ?? 0,
                                                                                    requisicao?.IdProduto ?? 0,
                                                                                    requisicao?.Quantidade ?? 0,
                                                                                    IdEmpresa);

                return ApiOk();
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }
        }

        [Authorize]
        [Route("api/v1/atividade-estoque/conferencia-399-400")]
        [HttpPost]
        public async Task<IHttpActionResult> FinalizarConferenciaProdutoForaLinha(FinalizarConferenciaProdutoForaLinhaRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                await _atividadeEstoqueService.FinalizarConferenciaProdutoForaLinha(requisicao?.Corredor ?? 0,
                                                                                    requisicao?.IdEnderecoArmazenagem ?? 0,
                                                                                    requisicao?.IdProduto ?? 0,
                                                                                    requisicao?.Quantidade,
                                                                                    IdEmpresa,
                                                                                    IdUsuario);

                return ApiOk();
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }
        }

        [Authorize]
        [Route("api/v1/atividade-estoque/conferencia-endereco")]
        [HttpPost]
        public async Task<IHttpActionResult> FinalizarConferenciaEndereco(FinalizarConferenciaEnderecoRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                await _atividadeEstoqueService.FinalizarConferenciaEndereco(requisicao, IdEmpresa, IdUsuario);

                return ApiOk();
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }
        }
    }
}
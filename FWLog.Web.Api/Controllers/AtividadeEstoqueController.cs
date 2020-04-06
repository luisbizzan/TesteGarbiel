﻿using FWLog.Data;
using FWLog.Services.Model.AtividadeEstoque;
using FWLog.Services.Services;
using FWLog.Web.Api.Models.AtividadeEstoque;
using System;
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
            catch
            {
                throw;
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
            catch
            {
                throw;
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
            catch
            {
                throw;
            }

            return ApiOk();
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
        [Route("api/v1/atividade-estoque/pesquisar")]
        [HttpGet]
        public IHttpActionResult PesquisarAtividade()
        {
            var resposta = new AtividadesEstoqueResposta
            {
                Lista = _atividadeEstoqueService.PesquisarAtividade(IdEmpresa, IdUsuario)
            };

            return ApiOk(resposta);
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
        public IHttpActionResult FinalizarConferenciaProdutoForaLinha(FinalizarConferenciaProdutoForaLinhaRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                _atividadeEstoqueService.FinalizarConferenciaProdutoForaLinhaRequisicao(requisicao?.Corredor ?? 0,
                                                                                    requisicao?.IdEnderecoArmazenagem ?? 0,
                                                                                    requisicao?.IdProduto ?? 0,
                                                                                    requisicao?.Quantidade,
                                                                                    IdEmpresa);

                return ApiOk();
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }
        }
    }
}
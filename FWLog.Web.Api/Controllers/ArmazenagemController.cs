﻿using FWLog.Services.Model.Armazenagem;
using FWLog.Services.Services;
using FWLog.Web.Api.Models.Armazenagem;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class ArmazenagemController : ApiBaseController
    {
        private readonly ArmazenagemService _armazenagemService;

        public ArmazenagemController(ArmazenagemService armazenagemService)
        {
            _armazenagemService = armazenagemService;
        }

        [Route("api/v1/armazenagem/instalar/validar-lote/{idLote}")]
        [HttpGet]
        public IHttpActionResult ValidarLoteInstalacao(long idLote)
        {
            try
            {
                var validarLoteRequisicao = new ValidarLoteInstalacaoRequisicao
                {
                    IdLote = idLote,
                    IdEmpresa = IdEmpresa
                };

                _armazenagemService.ValidarLoteInstalacao(validarLoteRequisicao);
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

        [Route("api/v1/armazenagem/instalar/validar-lote-produto")]
        [HttpPost]
        public IHttpActionResult ValidarLoteProdutoInstalacao(ValidarLoteProdutoInstalacaoModelRequisicao requisicao)
        {
            try
            {
                var validarProdutoRequisicao = new ValidarLoteProdutoInstalacaoRequisicao
                {
                    IdLote = requisicao.IdLote,
                    IdProduto = requisicao.IdProduto,
                    IdEmpresa = IdEmpresa
                };

                _armazenagemService.ValidarLoteProdutoInstalacao(validarProdutoRequisicao);
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

        [Route("api/v1/armazenagem/instalar/validar-quantidade")]
        [HttpPost]
        public IHttpActionResult ValidarQuantidadeInstalacao(ValidarQuantidadeInstalacaoModelRequisicao requisicao)
        {
            try
            {
                var validarQuantidadeRequisicao = new ValidarQuantidadeInstalacaoRequisicao
                {
                    IdLote = requisicao.IdLote,
                    IdProduto = requisicao.IdProduto,
                    Quantidade = requisicao.Quantidade,
                    IdEmpresa = IdEmpresa
                };

                _armazenagemService.ValidarQuantidadeInstalacao(validarQuantidadeRequisicao);
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

        [Route("api/v1/armazenagem/instalar/validar-endereco")]
        [HttpPost]
        public IHttpActionResult ValidarEnderecoInstalacao(ValidarEnderecoInstalacaoModelRequisicao requisicao)
        {
            try
            {
                var validarEnderecoRequisicao = new ValidarEnderecoInstalacaoRequisicao
                {
                    IdLote = requisicao.IdLote,
                    IdProduto = requisicao.IdProduto,
                    Quantidade = requisicao.Quantidade,
                    IdEmpresa = IdEmpresa,
                    IdEnderecoArmazenagem = requisicao.IdEnderecoArmazenagem
                };

                _armazenagemService.ValidarEnderecoInstalacao(validarEnderecoRequisicao);
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

        [Route("api/v1/armazenagem/instalar")]
        [HttpPost]
        public async Task<IHttpActionResult> InstalarVolumeLote(InstalarVolumeLoteModelRequisicao requisicao)
        {
            try
            {
                var instalarVolumeLoteRequisicao = new InstalarVolumeLoteRequisicao
                {
                    IdLote = requisicao.IdLote,
                    IdProduto = requisicao.IdProduto,
                    Quantidade = requisicao.Quantidade,
                    IdEmpresa = IdEmpresa,
                    IdEnderecoArmazenagem = requisicao.IdEnderecoArmazenagem,
                    IdUsuarioInstalacao = IdUsuario
                };

                await _armazenagemService.InstalarVolumeLote(instalarVolumeLoteRequisicao);
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

        [Route("api/v1/armazenagem/retirar/validar-endereco/{idEndereco}")]
        [HttpPost]
        public IHttpActionResult ValidarEnderecoRetirar(long idEndereco)
        {
            try
            {
                _armazenagemService.ValidarEnderecoRetirar(idEndereco);
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
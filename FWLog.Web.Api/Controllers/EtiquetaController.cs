using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Model.Etiquetas;
using FWLog.Services.Services;
using FWLog.Web.Api.Models.Etiqueta;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class EtiquetaController : ApiBaseController
    {
        private readonly EtiquetaService _etiquetaService;

        public EtiquetaController(EtiquetaService etiquetaService)
        {
            _etiquetaService = etiquetaService;
        }

        [HttpPost]
        [Route("api/v1/etiqueta/endereco/imprimir")]
        public IHttpActionResult ImprimirEtiquetaEndereco(ImprimirEtiquetaEnderecoRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                var requisicaoServico = new ImprimirEtiquetaEnderecoRequest
                {
                    IdEnderecoArmazenagem = requisicao.IdEnderecoArmazenagem,
                    IdImpressora = requisicao.IdImpressora,
                    IdUsuario = IdUsuario,
                    IdEmpresa = IdEmpresa
                };

                _etiquetaService.ImprimirEtiquetaEndereco(requisicaoServico);
            }
            catch (BusinessException ex)
            {
                return ApiInternalServerErrror("Erro na impressora", ex);
            }
            catch
            {
                throw;
            }

            return ApiOk();
        }

        [HttpPost]
        [Route("api/v1/etiqueta/picking/imprimir")]
        public IHttpActionResult ImprimirEtiquetaPicking(ImprimirEtiquetaPickingRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                var requisicaoServico = new ImprimirEtiquetaPickingRequest
                {
                    IdEnderecoArmazenagem = requisicao.IdEnderecoArmazenagem,
                    IdProduto = requisicao.IdProduto,
                    IdImpressora = requisicao.IdImpressora
                };

                _etiquetaService.ImprimirEtiquetaPicking(requisicaoServico);

                return ApiOk();
            }
            catch (BusinessException ex)
            {
                return ApiInternalServerErrror("Erro na impressora", ex);
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/v1/etiqueta/lote/imprimir")]
        public IHttpActionResult ImprimirEtiquetaLote(ImprimirEtiquetaLoteRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }
            else
            {
                try
                {
                    var request = new ImprimirEtiquetaLoteRequest
                    {
                        IdLote = requisicao.IdLote,
                        IdProduto = requisicao.IdProduto,
                        IdImpressora = requisicao.IdImpressora,
                        QuantidadeProdutos = requisicao.QuantidadeProdutos,
                        QuantidadeEtiquetas = requisicao.QuantidadeEtiquetas,
                        IdUsuario = IdUsuario,
                        IdEmpresa = IdEmpresa
                    };

                    _etiquetaService.ValidarEImprimirEtiquetaLote(request);
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

        [HttpPost]
        [Route("api/v1/etiqueta/produto/imprimir")]
        public IHttpActionResult ImprimirEtiquetaProduto(ImprimirEtiquetaProdutoRequisicao requisicao)
        {
            if (requisicao.IdImpressaoItem == (int)ImpressaoItemEnum.EtiquetaAvulso)
            {
                ModelState.Remove(nameof(requisicao.ReferenciaProduto));
            }

            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }
            else
            {
                try
                {
                    switch (requisicao.IdImpressaoItem)
                    {
                        case (int)ImpressaoItemEnum.EtiquetaAvulso:
                            _etiquetaService.ImprimirEtiquetaAvulso(new ImprimirEtiquetaAvulsoRequest
                            {
                                IdEmpresa = IdEmpresa,
                                IdImpressora = requisicao.IdImpressora,
                                QuantidadeEtiquetas = requisicao.QuantidadeEtiquetas,
                                IdUsuario = IdUsuario,
                                ReferenciaProduto = requisicao.ReferenciaProduto
                            });
                            break;
                        case (int)ImpressaoItemEnum.EtiquetaIndividual:
                            _etiquetaService.ImprimirEtiquetaPeca(new ImprimirEtiquetaProdutoBase
                            {
                                IdEmpresa = IdEmpresa,
                                IdImpressora = requisicao.IdImpressora,
                                ReferenciaProduto = requisicao.ReferenciaProduto,
                                QuantidadeEtiquetas = requisicao.QuantidadeEtiquetas,
                                IdUsuario = IdUsuario
                            });
                            break;
                        case (int)ImpressaoItemEnum.EtiquetaPersonalizada:
                            _etiquetaService.ImprimirEtiquetaPersonalizada(new ImprimirEtiquetaProdutoBase
                            {
                                IdEmpresa = IdEmpresa,
                                IdImpressora = requisicao.IdImpressora,
                                ReferenciaProduto = requisicao.ReferenciaProduto,
                                QuantidadeEtiquetas = requisicao.QuantidadeEtiquetas,
                                IdUsuario = IdUsuario
                            });
                            break;
                        default:
                            return ApiBadRequest("Tipo da impressão não existe.");
                    }
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

        [HttpPost]
        [Route("api/v1/etiqueta/picking/validar-produto-endereco")]
        public IHttpActionResult ValidarProdutoOuEnderecoPicking(ValidarEnderecoPickingRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                var request = new ValidarEnderecoPickingRequest
                {
                    referenciaProdutoOuEndereco = requisicao.ReferenciaProdutoOuEndereco,
                    IdEmpresa = IdEmpresa
                };

               var resposta = _etiquetaService.ValidarProdutoOuEnderecoPicking(request);

                return ApiOk(resposta);

            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }
            catch
            {
                throw;
            }
        }
    }
}
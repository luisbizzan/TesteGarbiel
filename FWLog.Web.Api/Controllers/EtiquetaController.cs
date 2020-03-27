using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Model.Coletor;
using FWLog.Services.Model.Etiquetas;
using FWLog.Services.Services;
using FWLog.Web.Api.Models.Etiqueta;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class EtiquetaController : ApiBaseController
    {
        private readonly EtiquetaService _etiquetaService;
        private readonly UnitOfWork _unitOfWork;
        private readonly ColetorHistoricoService _coletorHistoricoService;

        public EtiquetaController(UnitOfWork unitOfWork, EtiquetaService etiquetaService, ColetorHistoricoService coletorHistoricoService)
        {
            _unitOfWork = unitOfWork;
            _etiquetaService = etiquetaService;
            _coletorHistoricoService = coletorHistoricoService;
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
                    IdImpressora = requisicao.IdImpressora
                };

                var imprimriEtiquetaEnderecoResponse = _etiquetaService.ImprimirEtiquetaEndereco(requisicaoServico);

                var gravarHistoricoColetorRequisicao = new GravarHistoricoColetorRequisicao
                {
                    IdColetorAplocacao = ColetorAplicacaoEnum.Armazenagem,
                    IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.InstalarProduto,
                    Descricao = $"Iprimiu a etiqueita de endereço com o código {imprimriEtiquetaEnderecoResponse.EnderecoArmazenagem.Codigo}",
                    IdEmpresa = IdEmpresa,
                    IdUsuario = IdUsuario
                };

                _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisicao);

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

                    var imprimirEtiquetaLoteReponse = _etiquetaService.ValidarEImprimirEtiquetaLote(request);

                    var gravarHistoricoColetorRequisciao = new GravarHistoricoColetorRequisicao
                    {
                        IdColetorAplocacao = ColetorAplicacaoEnum.Armazenagem,
                        IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.ImprimirEtiqueta,
                        Descricao = $"Imprimiu a etiqueta do lote {request.IdLote} do(s) produto() {imprimirEtiquetaLoteReponse.Produto.Referencia}",
                        IdEmpresa = IdEmpresa,
                        IdUsuario = IdUsuario
                    };

                    _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisciao);

                    return ApiOk();
                }
                catch (BusinessException ex)
                {
                    return ApiBadRequest(ex.Message);
                }
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
                    var gravarHistoricoColetorRequisciao = new GravarHistoricoColetorRequisicao
                    {
                        IdColetorAplocacao = ColetorAplicacaoEnum.Armazenagem,
                        IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.ImprimirEtiqueta,
                        IdEmpresa = IdEmpresa,
                        IdUsuario = IdUsuario
                    };

                    switch (requisicao.IdImpressaoItem)
                    {
                        case (int)ImpressaoItemEnum.EtiquetaAvulso:
                            _etiquetaService.ImprimirEtiquetaAvulso(new ImprimirEtiquetaAvulsoRequest
                            {
                                IdEmpresa = IdEmpresa,
                                IdImpressora = requisicao.IdImpressora,
                                QuantidadeEtiquetas = requisicao.QuantidadeEtiquetas
                            });
                            gravarHistoricoColetorRequisciao.Descricao = $"Imprimiu {requisicao.QuantidadeEtiquetas} etiqueta(s) avulsa(s) do produto {requisicao.ReferenciaProduto}";
                            _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisciao);
                            break;
                        case (int)ImpressaoItemEnum.EtiquetaIndividual:
                            _etiquetaService.ImprimirEtiquetaPeca(new ImprimirEtiquetaProdutoBase
                            {
                                IdEmpresa = IdEmpresa,
                                IdImpressora = requisicao.IdImpressora,
                                ReferenciaProduto = requisicao.ReferenciaProduto,
                                QuantidadeEtiquetas = requisicao.QuantidadeEtiquetas
                            });
                            gravarHistoricoColetorRequisciao.Descricao = $"Imprimiu {requisicao.QuantidadeEtiquetas} etiqueta(s) individual(ais) do produto {requisicao.ReferenciaProduto}";
                            _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisciao);
                            break;
                        case (int)ImpressaoItemEnum.EtiquetaPersonalizada:
                            _etiquetaService.ImprimirEtiquetaPersonalizada(new ImprimirEtiquetaProdutoBase
                            {
                                IdEmpresa = IdEmpresa,
                                IdImpressora = requisicao.IdImpressora,
                                ReferenciaProduto = requisicao.ReferenciaProduto,
                                QuantidadeEtiquetas = requisicao.QuantidadeEtiquetas
                            });
                            gravarHistoricoColetorRequisciao.Descricao = $"Imprimiu {requisicao.QuantidadeEtiquetas} etiqueta(s) personalizada(s) do produto {requisicao.ReferenciaProduto}";
                            _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisciao);
                            break;
                        default:
                            return ApiBadRequest("Tipo da impressão não existe.");
                    }

                    return ApiOk();
                }
                catch (BusinessException ex)
                {
                    return ApiBadRequest(ex.Message);
                }
            }
        }

        [HttpPost]
        [Route("api/v1/etiqueta/picking/validar-endereco")]
        public IHttpActionResult ValidarEnderecoPicking(ValidarEnderecoPickingRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }
            else
            {
                try
                {
                    var request = new ValidarEnderecoPickingRequest
                    {
                        IdEnderecoArmazenagem = requisicao.IdEnderecoArmazenagem,
                    };

                    _etiquetaService.ValidarEnderecoPicking(request);

                    return ApiOk();

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
}
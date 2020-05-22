using FWLog.Data;
using FWLog.Services.Model.Etiquetas;
using FWLog.Services.Model.LogEtiquetagem;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.EtiquetaCtx;
using log4net;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class EtiquetaController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly EtiquetaService _etiquetaService;
        private readonly LogEtiquetagemService _logEtiquetagemService;
        private ILog _log;

        public EtiquetaController(UnitOfWork unitOfWork, LogEtiquetagemService logEtiquetagemService, EtiquetaService etiquetaService, ILog log)
        {
            _unitOfWork = unitOfWork;
            _etiquetaService = etiquetaService;
            _logEtiquetagemService = logEtiquetagemService;
            _log = log;
        }

        #region Lote

        [HttpGet]
        public ActionResult Lote()
        {
            return View(new LoteEtiquetaViewModel());
        }

        [HttpPost]
        public JsonResult LoteImprimir(LoteEtiquetaViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Não foi possível solicitar impressão."
                    });
                }

                //Instância a lote produto.
                var loteProduto = _unitOfWork.LoteProdutoRepository.ConsultarPorLote(viewModel.NroLote.Value);

                if (loteProduto == null)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Não foi possível consultar o saldo do lote. Por favor, tente nvoamente!"
                    });
                }

                var quantidadeTotalProduto = viewModel.QtdCaixas * viewModel.QtdPorCaixa;

                //Verifica se o saldo do produto no lote é menor que a quantidade informada.
                if (loteProduto.Saldo < quantidadeTotalProduto)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "O saldo do produto no lote é menor que a quantidade informada. Por favor, tente novamente!"
                    });
                }

                var produto = _unitOfWork.ProdutoRepository.GetById(viewModel.IdProduto);

                var request = new ImprimirEtiquetaArmazenagemVolume
                {
                    NroLote = viewModel.NroLote.GetValueOrDefault(),
                    QuantidadeEtiquetas = viewModel.QtdCaixas.GetValueOrDefault(),
                    QuantidadePorCaixa = viewModel.QtdPorCaixa.GetValueOrDefault(),
                    ReferenciaProduto = produto.Referencia,
                    Usuario = _unitOfWork.PerfilUsuarioRepository.GetByUserId(User.Identity.GetUserId())?.Nome,
                    IdImpressora = viewModel.IdImpressora.GetValueOrDefault(),
                    IdEmpresa = IdEmpresa
                };

                _etiquetaService.ImprimirEtiquetaArmazenagemVolume(request);

                // Lote: a quantidade salva é a quantidade de caixas/volume do lote.
                var logEtiquetagem = new LogEtiquetagem
                {
                    //IdTipoEtiquetagem = viewModel.TipoEtiquetagem,
                    IdTipoEtiquetagem = Data.Models.TipoEtiquetagemEnum.Lote.GetHashCode(),
                    IdEmpresa = IdEmpresa,
                    Quantidade = viewModel.QtdCaixas.Value,
                    IdUsuario = User.Identity.GetUserId()
                };

                _logEtiquetagemService.Registrar(logEtiquetagem);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Impressão enviada com sucesso."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Ocorreu um erro na impressão."
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult LoteValidaImpressao(LoteEtiquetaViewModel viewModel)
        {
            try
            {
                if (!(viewModel.QtdPorCaixa > 0))
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Quantidade por Caixas deve ser maior que zero."
                    });
                }

                if (!(viewModel.QtdCaixas > 0))
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Quantidade de Caixas deve ser maior que zero."
                    });
                }

                if (string.IsNullOrEmpty(viewModel.DescricaoProduto))
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Referência do Produto não pode ser vazio."
                    });
                }

                bool existeLote = _unitOfWork.LoteRepository.Existe(x => x.IdLote == viewModel.NroLote);

                if (!existeLote)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Lote não encontrado."
                    });
                }

                bool existeLoteProduto = _unitOfWork.LoteConferenciaRepository.ObterPorProduto(viewModel.NroLote.GetValueOrDefault(), viewModel.IdProduto).Any();

                if (!existeLoteProduto)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Nenhum Lote com este Produto encontrado."
                    });
                }

                return Json(new AjaxGenericResultModel
                {
                    Success = true
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Ocorreu na validação da impressão."
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Recebimento

        [HttpGet]
        public ActionResult Recebimento()
        {
            return View(new RecebimentoEtiquetaViewModel());
        }

        [HttpPost]
        public JsonResult RecebimentoImprimir(RecebimentoEtiquetaViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Não foi possível solicitar impressão."
                    });
                }

                _etiquetaService.ImprimirEtiquetaVolumeRecebimento(viewModel.NroLote.GetValueOrDefault(), viewModel.IdImpressora.GetValueOrDefault(), viewModel.NroVolume.GetValueOrDefault(), 1);

                // Lote: a quantidade salva é a quantidade de caixas/volume do lote.
                var logEtiquetagem = new LogEtiquetagem
                {
                    //IdTipoEtiquetagem = viewModel.TipoEtiquetagem,
                    IdTipoEtiquetagem = Data.Models.TipoEtiquetagemEnum.Recebimento.GetHashCode(),
                    IdEmpresa = IdEmpresa,
                    Quantidade = 1,
                    IdUsuario = User.Identity.GetUserId()
                };

                _logEtiquetagemService.Registrar(logEtiquetagem);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Impressão enviada com sucesso."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Ocorreu um erro na impressão."
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult RecebimentoValidaImpressao(RecebimentoEtiquetaViewModel viewModel)
        {
            try
            {
                if (!(viewModel.NroVolume > 0))
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Nro do Volume deve ser maior que zero."
                    });
                }

                var lote = _unitOfWork.LoteRepository.GetById(viewModel.NroLote.GetValueOrDefault());

                if (lote == null)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Lote não encontrado."
                    });
                }

                if (viewModel.NroVolume > lote.QuantidadeVolume)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Nro do Volume não pode ser maior que a quantidade de volumes do lote."
                    });
                }

                return Json(new AjaxGenericResultModel
                {
                    Success = true
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Ocorreu na validação da impressão."
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Loacação
        [HttpGet]
        public ActionResult Locacao()
        {
            return View(new LocacaoEtiquetaViewModel());
        }

        [HttpPost]
        public JsonResult LocacaoImprimir(LocacaoEtiquetaViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Não foi possível solicitar impressão."
                    });
                }

                //Captura o ponto de armazenagem.
                var pontoArmazenagem = _unitOfWork.PontoArmazenagemRepository.GetById(viewModel.IdPontoArmazenagem);

                //Captura os enderços filtrando por nível, ponto e empresa.
                var listaEnderecos = _unitOfWork.EnderecoArmazenagemRepository.BuscarPorNivelEPontoArmazenagem(
                    pontoArmazenagem.IdNivelArmazenagem, pontoArmazenagem.IdPontoArmazenagem, IdEmpresa);

                //Filtra por corredor e vertical.
                listaEnderecos = listaEnderecos.Where(x => x.Corredor == viewModel.Corredor &&
                (viewModel.VerticalInicio.HasValue == false || x.Vertical >= viewModel.VerticalInicio.Value) &&
                (viewModel.VerticalFim.HasValue == false || x.Vertical <= viewModel.VerticalFim.Value)).ToList();

                var impressaoItem = _unitOfWork.ImpressaoItemRepository.Obter(viewModel.TipoEtiqueta);

                //Se a etiqueta for picking, filtra os endereços de picking.
                if (impressaoItem.IdImpressaoItem == Data.Models.ImpressaoItemEnum.EtiquetaPicking)
                    listaEnderecos = listaEnderecos.Where(x => x.IsPicking == true).ToList();

                int quantidadeEtiqueta = 0;

                if (viewModel.TipoEtiqueta == (int)Data.Models.ImpressaoItemEnum.EtiquetaPicking)
                {
                    foreach (var item in listaEnderecos)
                    {
                        var produtoInstalado = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEndereco(item.IdEnderecoArmazenagem);
                        
                        if (produtoInstalado != null)
                        {
                            if (viewModel.TamanhoEtiqueta == 1)
                            {
                                _etiquetaService.ImprimirEtiquetaPicking(new ImprimirEtiquetaPickingRequest()
                                {
                                    IdEnderecoArmazenagem = item.IdEnderecoArmazenagem,
                                    IdProduto = produtoInstalado.IdProduto,
                                    IdImpressora = viewModel.IdImpressora.Value,
                                    QuantidadeEtiquetas = 1
                                });
                            }
                            else
                            {
                                _etiquetaService.ImprimirEtiquetaFilete(produtoInstalado.IdProduto,
                                    item.IdEnderecoArmazenagem, viewModel.IdImpressora.Value);
                            }

                            quantidadeEtiqueta++;
                        }
                    }

                    var logEtiquetagem = new LogEtiquetagem
                    {
                        IdTipoEtiquetagem = Data.Models.TipoEtiquetagemEnum.Picking.GetHashCode(),
                        IdEmpresa = IdEmpresa,
                        Quantidade = quantidadeEtiqueta,
                        IdUsuario = User.Identity.GetUserId()
                    };

                    _logEtiquetagemService.Registrar(logEtiquetagem);
                }
                else
                {
                    foreach (var item in listaEnderecos)
                    {
                        _etiquetaService.ImprimirEtiquetaEndereco(new ImprimirEtiquetaEnderecoRequest()
                        {
                            IdEmpresa = IdEmpresa,
                            IdEnderecoArmazenagem = item.IdEnderecoArmazenagem,
                            IdImpressora = viewModel.IdImpressora.Value,
                            IdUsuario = IdUsuario,
                            QuantidadeEtiquetas = 1,
                            TipoImpressao = viewModel.TamanhoEtiqueta == 1 ? EtiquetaEnderecoTipoImpressao.NORMAL_90_70 : EtiquetaEnderecoTipoImpressao.FILETE_104_24
                            });

                        quantidadeEtiqueta++;
                    }

                    var logEtiquetagem = new LogEtiquetagem
                    {
                        IdTipoEtiquetagem = Data.Models.TipoEtiquetagemEnum.Picking.GetHashCode(),
                        IdEmpresa = IdEmpresa,
                        Quantidade = quantidadeEtiqueta,
                        IdUsuario = User.Identity.GetUserId()
                    };

                    _logEtiquetagemService.Registrar(logEtiquetagem);
                }
                   

               return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Impressão enviada com sucesso."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Ocorreu um erro na impressão."
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult LocacaoValidaImpressao(LocacaoEtiquetaViewModel viewModel)
        {
            try
            {
                if (!(viewModel.Corredor > 0))
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "O corredor deve ser maior que zero."
                    });
                }

                var pontoArmazenagem = _unitOfWork.PontoArmazenagemRepository.GetById(viewModel.IdPontoArmazenagem);

                if (pontoArmazenagem == null)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Ponto de armazenagem não encontrado."
                    });
                }

                var impressaoItem = _unitOfWork.ImpressaoItemRepository.Obter(viewModel.TipoEtiqueta);

                if (impressaoItem == null)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Tipo de impressão não encontrado."
                    });
                }

                var listaEnderecos = _unitOfWork.EnderecoArmazenagemRepository.BuscarPorNivelEPontoArmazenagem(
                    pontoArmazenagem.IdNivelArmazenagem, pontoArmazenagem.IdPontoArmazenagem, IdEmpresa);

                listaEnderecos = listaEnderecos.Where(x => x.Corredor == viewModel.Corredor &&
                (viewModel.VerticalInicio.HasValue == false || x.Vertical >= viewModel.VerticalInicio.Value) &&
                (viewModel.VerticalFim.HasValue == false || x.Vertical <= viewModel.VerticalFim.Value)).ToList();

                if (impressaoItem.IdImpressaoItem == Data.Models.ImpressaoItemEnum.EtiquetaPicking)
                    listaEnderecos = listaEnderecos.Where(x => x.IsPicking == true).ToList();

                if (listaEnderecos.Count == 0)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Nenhum endereço encontrado para os parâmetros informados."
                    });
                }


                return Json(new AjaxGenericResultModel
                {
                    Success = true
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Ocorreu na validação da impressão."
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}
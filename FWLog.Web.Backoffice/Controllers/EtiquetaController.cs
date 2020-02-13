using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Services.Model.Etiquetas;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.EtiquetaCtx;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;
using FWLog.Services.Model.LogEtiquetagem;

namespace FWLog.Web.Backoffice.Controllers
{
    public class EtiquetaController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly EtiquetaService _etiquetaService;
        private readonly LogEtiquetagemService _logEtiquetagemService;
        private readonly ApplicationLogService _applicationLogService;

        public EtiquetaController(UnitOfWork unitOfWork, LogEtiquetagemService logEtiquetagemService, EtiquetaService etiquetaService, ApplicationLogService applicationLogService)
        {
            _unitOfWork = unitOfWork;
            _etiquetaService = etiquetaService;
            _logEtiquetagemService = logEtiquetagemService;
            _applicationLogService = applicationLogService;
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
                _applicationLogService.Error(ApplicationEnum.BackOffice, ex);

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
                _applicationLogService.Error(ApplicationEnum.BackOffice, ex);

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

                _etiquetaService.ImprimirEtiquetaVolumeRecebimento(viewModel.NroLote.GetValueOrDefault(), viewModel.IdImpressora.GetValueOrDefault(), viewModel.Quantide.GetValueOrDefault());

                // Lote: a quantidade salva é a quantidade de caixas/volume do lote.
                var logEtiquetagem = new LogEtiquetagem
                {
                    //IdTipoEtiquetagem = viewModel.TipoEtiquetagem,
                    IdTipoEtiquetagem = Data.Models.TipoEtiquetagemEnum.Recebimento.GetHashCode(),
                    IdEmpresa = IdEmpresa,
                    Quantidade = viewModel.Quantide.Value,
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
                _applicationLogService.Error(ApplicationEnum.BackOffice, ex);

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
                if (!(viewModel.Quantide > 0))
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Quantidade deve ser maior que zero."
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

                if (viewModel.Quantide > lote.QuantidadeVolume)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Quantidade não pode ser maior que a quantidade de volumes do lote."
                    });
                }

                return Json(new AjaxGenericResultModel
                {
                    Success = true
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _applicationLogService.Error(ApplicationEnum.BackOffice, ex);

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
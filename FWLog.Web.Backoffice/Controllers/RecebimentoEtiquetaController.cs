using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Services.Model.Etiquetas;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.RecebimentoNotaCtx;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class RecebimentoEtiquetaController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly EtiquetaService _etiquetaService;
        private readonly ApplicationLogService _applicationLogService;

        public RecebimentoEtiquetaController(UnitOfWork unitOfWork, EtiquetaService etiquetaService, ApplicationLogService applicationLogService)
        {
            _unitOfWork = unitOfWork;
            _etiquetaService = etiquetaService;
            _applicationLogService = applicationLogService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(new RecebimentoEtiquetaViewModel());
        }

        [HttpPost]
        public JsonResult Imprimir(RecebimentoEtiquetaViewModel viewModel)
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

                var request = new ImprimirEtiquetaArmazenagemVolume
                {
                    NroLote = viewModel.NroLote.GetValueOrDefault(),
                    QuantidadeEtiquetas = viewModel.QtdCaixas.GetValueOrDefault(),
                    QuantidadePorCaixa = viewModel.QtdPorCaixa.GetValueOrDefault(),
                    ReferenciaProduto = viewModel.ReferenciaProduto,
                    Usuario = _unitOfWork.PerfilUsuarioRepository.GetByUserId(User.Identity.GetUserId())?.Nome,
                    IdImpressora = viewModel.IdImpressora.GetValueOrDefault(),
                    IdEmpresa = IdEmpresa
                };

                _etiquetaService.ImprimirEtiquetaArmazenagemVolume(request);

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
        public JsonResult ValidaImpressao(RecebimentoEtiquetaViewModel viewModel)
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

                if (string.IsNullOrEmpty(viewModel.ReferenciaProduto))
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Referência do Produto não pode ser vazio."
                    });
                }

                long? idProduto = _unitOfWork.ProdutoRepository.Todos().FirstOrDefault(x => x.Referencia.ToUpper() == viewModel.ReferenciaProduto.ToUpper())?.IdProduto;

                if (idProduto == null)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Produto não encontrado."
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

                bool existeLoteProduto = _unitOfWork.LoteConferenciaRepository.ObterPorProduto(viewModel.NroLote.GetValueOrDefault(), idProduto.GetValueOrDefault()).Any();

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
    }
}
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Services.Model.LogEtiquetagem;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.RecebimentoEtiquetaIndividualPersonalizadaCtx;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class RecebimentoEtiquetaIndividualPersonalizadaController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly LogEtiquetagemService _logEtiquetagemService;
        private readonly EtiquetaService _etiquetaService;
        private readonly ApplicationLogService _applicationLogService;

        public RecebimentoEtiquetaIndividualPersonalizadaController(UnitOfWork unitOfWork, LogEtiquetagemService logEtiquetagemService, EtiquetaService etiquetaService, ApplicationLogService applicationLogService)
        {
            _unitOfWork = unitOfWork;
            _logEtiquetagemService = logEtiquetagemService;
            _etiquetaService = etiquetaService;
            _applicationLogService = applicationLogService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(new RecebimentoEtiquetaIndividualPersonalizadaViewModel());
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RecebimentoEtiquetaIndividualEPersonalizada.Imprimir)]
        public JsonResult Imprimir(RecebimentoEtiquetaIndividualPersonalizadaViewModel viewModel)
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

                //var request = new ImprimirEtiquetaArmazenagemVolume
                //{
                //    NroLote = viewModel.NroLote.GetValueOrDefault(),
                //    QuantidadeEtiquetas = viewModel.QtdCaixas.GetValueOrDefault(),
                //    QuantidadePorCaixa = viewModel.QtdPorCaixa.GetValueOrDefault(),
                //    ReferenciaProduto = viewModel.ReferenciaProduto,
                //    Usuario = _unitOfWork.PerfilUsuarioRepository.GetByUserId(User.Identity.GetUserId())?.Nome,
                //    IdImpressora = viewModel.IdImpressora.GetValueOrDefault(),
                //    IdEmpresa = IdEmpresa
                //};

                //_etiquetaService.ImprimirEtiquetaArmazenagemVolume(request);

                var logEtiquetagem = new LogEtiquetagem
                {
                    IdTipoEtiquetagem = viewModel.TipoEtiquetagem,
                    IdEmpresa = IdEmpresa,
                    IdProduto = viewModel.IdProduto.Value,
                    Quantidade = viewModel.Quantidade.Value,
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
        public JsonResult ValidaImpressao(RecebimentoEtiquetaIndividualPersonalizadaViewModel viewModel)
        {
            try
            {
                if (!(viewModel.Quantidade > 0))
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Quantidade deve ser maior que zero."
                    });
                }

                if (viewModel.IdProduto == null)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Referência inválida. Por favor, tente novamente!"
                    });
                }

                var produto = _unitOfWork.ProdutoRepository.Todos().FirstOrDefault(x => x.IdProduto == viewModel.IdProduto);

                if (produto == null)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Referência não encontrada. Por favor, tente novamente!"
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
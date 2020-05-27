using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.GarantiaEtiquetaCtx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class GarantiaEtiquetaController : BOBaseController
    {
        #region [Variáveis Sessão]
        private readonly UnitOfWork _unitOfWork;
        private readonly GarantiaEtiquetaService _garantiaEtiquetaService;

        public GarantiaEtiquetaController(UnitOfWork unitOfWork, GarantiaEtiquetaService garantiaEtiquetaService)
        {
            _unitOfWork = unitOfWork;
            _garantiaEtiquetaService = garantiaEtiquetaService;
        }
        #endregion

        #region Views
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region Enviar Etiqueta para Impressão
        [HttpPost]
        public JsonResult ProcessarImpressaoEtiqueta(GarantiaEtiquetaViewModel EtiquetaImpressao)
        {
            try
            {
                #region Validações
                Func<ViewResult> errorView = () => { return View(EtiquetaImpressao); };

                if (!ModelState.IsValid)
                    throw new Exception(ModelState.Values.Where(x => x.Errors.Count > 0).Aggregate("", (current, s) => current + (s.Errors[0].ErrorMessage + "<br/>")));

                if (EtiquetaImpressao.EtiquetaImpressaoIds.Count.Equals(0))
                    throw new Exception("Nenhuma etiqueta selecionada para Impressão!");
                #endregion

                _garantiaEtiquetaService.ProcessarImpressaoEtiqueta(new GarantiaEtiqueta.DocumentoImpressao()
                {
                    IdsEtiquetasImprimir = EtiquetaImpressao.EtiquetaImpressaoIds
                });

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Etiqueta enviado para impressão com sucesso."
                });
            }
            catch (Exception ex)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
        #endregion

    }
}
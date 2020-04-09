using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.GarantiaConfiguracaoCtx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class GarantiaConfiguracaoController : BOBaseController
    {
        #region variáveis sessão
        private readonly UnitOfWork _unitOfWork;
        private readonly GeralService _geralService;

        public GarantiaConfiguracaoController(UnitOfWork unitOfWork, GeralService geralService)
        {
            _unitOfWork = unitOfWork;
            _geralService = geralService;
        }
        #endregion

        #region views
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region Fornecedor Quebra
        [HttpPost]
        public ActionResult IncluirFornecedorQuebra(GarantiaConfiguracaoViewModel model)
        {
            Func<ViewResult> errorView = () =>
            {
                return View(model);
            };

            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values.Where(x => x.Errors.Count > 0).Aggregate("", (current, s) => current + (s.Errors[0].ErrorMessage + "<br />"));
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = erros
                });
            }

            var _fornecedor = new GarantiaConfiguracao
            {
                Id = model.Id,
                Cod_Fornecedor = model.Cod_Fornecedor
            };

            try
            {
                //_geralService.InserirHistorico(item);
                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = Resources.CommonStrings.RegisterCreatedSuccessMessage
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
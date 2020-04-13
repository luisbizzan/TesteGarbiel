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
        private readonly GarantiaConfiguracaoService _garantiaConfigService;

        public GarantiaConfiguracaoController(UnitOfWork unitOfWork, GarantiaConfiguracaoService garantiaConfigService)
        {
            _unitOfWork = unitOfWork;
            _garantiaConfigService = garantiaConfigService;
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
        public ActionResult IncluirFornecedorQuebra(GarantiaConfiguracaoViewModel fornecedor)
        {
            try
            {
                Func<ViewResult> errorView = () => { return View(fornecedor); };

                if (!ModelState.IsValid)
                    throw new Exception(ModelState.Values.Where(x => x.Errors.Count > 0).Aggregate("", (current, s) => current + (s.Errors[0].ErrorMessage + "<br />")));

                _garantiaConfigService.IncluirFornecedorQuebra(new GarantiaConfiguracao() { Id = fornecedor.Id, Cod_Fornecedor = fornecedor.Cod_Fornecedor.ToUpper().Trim() });

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

        #region AutoComplete Fornecedor
        [HttpPost]
        public JsonResult AutoCompleteFornecedor(string valor)
        {
            try
            {
                if (String.IsNullOrEmpty(valor))
                    throw new Exception("Fornecedor não pode estar em branco!");

               var _fornecedores = _garantiaConfigService.AutoCompleteFornecedor(valor.ToUpper());

                return Json(new { Success = true, Fornecedores = _fornecedores }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }            
        }
        #endregion
    }
}
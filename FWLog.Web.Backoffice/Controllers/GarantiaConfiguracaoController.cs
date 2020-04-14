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
        #region [Variáveis Sessão]
        private readonly UnitOfWork _unitOfWork;
        private readonly GarantiaConfiguracaoService _garantiaConfigService;

        public GarantiaConfiguracaoController(UnitOfWork unitOfWork, GarantiaConfiguracaoService garantiaConfigService)
        {
            _unitOfWork = unitOfWork;
            _garantiaConfigService = garantiaConfigService;
        }
        #endregion

        #region Views
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region [Fornecedor Quebra] Incluir
        [HttpPost]
        public ActionResult FornecedorQuebraIncluir(GarantiaConfiguracaoViewModel fornecedor)
        {
            try
            {
                Func<ViewResult> errorView = () => { return View(fornecedor); };

                if (!ModelState.IsValid)
                    throw new Exception(ModelState.Values.Where(x => x.Errors.Count > 0).Aggregate("", (current, s) => current + (s.Errors[0].ErrorMessage + "<br />")));

                _garantiaConfigService.FornecedorQuebraIncluir(new GarantiaConfiguracao()
                {
                    Id = fornecedor.Id,
                    Cod_Fornecedor = String.IsNullOrEmpty(fornecedor.Cod_Fornecedor) ? String.Empty : fornecedor.Cod_Fornecedor.ToUpper().Trim(),
                    Codigos = fornecedor.Codigos
                });

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

        #region [Fornecedor Quebra] Excluir
        [HttpPost]
        public ActionResult FornecedorQuebraExcluir(int Id)
        {
            try
            {
                if (Id.Equals(0))
                    throw new Exception(String.Format("Id [{0}] inválido!", Id));

                _garantiaConfigService.FornecedorQuebraExcluir(Id);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = Resources.CommonStrings.RegisterDeletedSuccessMessage
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

        #region [Fornecedor Quebra] Listar
        [HttpGet]
        public JsonResult FornecedorQuebraListar()
        {
            try
            {
                var _lista = _garantiaConfigService.FornecedorQuebraListar();

                #region Formatar Valores para View
                _lista.ToList<GarantiaConfiguracao>().ForEach(delegate (GarantiaConfiguracao fornecedor)
                {
                    fornecedor.BotaoEvento =
                    String.Format("<button type=\"button\" class=\"btn btn-danger\" onclick=\"FornecedorQuebraExcluir({0});\"><i class=\"fa fa-trash-o\" alt=\"Excluir Fornecedor\"></i></button>", fornecedor.Id);
                });
                #endregion

                return Json(new { Success = true, Data = _lista.ToArray() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region [Fornecedor Quebra] AutoComplete
        [HttpPost]
        public JsonResult FornecedorQuebraAutoComplete(string valor)
        {
            try
            {
                if (String.IsNullOrEmpty(valor))
                    throw new Exception("Fornecedor não pode estar em branco!");

                var _fornecedores = _garantiaConfigService.FornecedorQuebraAutoComplete(valor.ToUpper());

                return Json(new { Success = true, Fornecedores = _fornecedores }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}
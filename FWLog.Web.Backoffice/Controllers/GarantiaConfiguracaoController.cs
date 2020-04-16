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

        #region [Genérico] Listar
        [HttpGet]
        public JsonResult RegistroListar(string TAG)
        {
            try
            {
                #region Validações
                if (String.IsNullOrEmpty(TAG))
                    throw new Exception(String.Concat("Obrigatório informar a Tag!"));

                if (!GarantiaConfiguracao.DicTagsValidas.Values.Contains(TAG))
                    throw new Exception(String.Format("Tag {0} inválida!", TAG));
                #endregion

                #region [Processamento] Formatar Valores para View 
                var _lista = _garantiaConfigService.RegistroListar(TAG);

                GarantiaConfiguracao.GridNome = GarantiaConfiguracao.DicTagGridNome.Where(w => w.Key.Equals(TAG, StringComparison.InvariantCulture)).FirstOrDefault().Value;
                GarantiaConfiguracao.GridColunas = (object[])GarantiaConfiguracao.DicTagGridColuna.Where(w => w.Key.Equals(TAG, StringComparison.InvariantCulture)).FirstOrDefault().Value;

                _lista.ToList<GarantiaConfiguracao>().ForEach(delegate (GarantiaConfiguracao Registro)
                {
                    Registro.BotaoEvento = String.Format(GarantiaConfiguracao.botaoExcluirTemplate, TAG, Registro.Id);
                });
                #endregion

                return Json(new
                {
                    Success = true,
                    GridColunas = GarantiaConfiguracao.GridColunas.ToArray(),
                    GridNome = GarantiaConfiguracao.GridNome,
                    Data = _lista.ToArray()
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region [Genérico] Excluir
        [HttpPost]
        public ActionResult RegistroExcluir(GarantiaConfiguracao Registro)
        {
            try
            {
                #region Validações
                if (Registro.Id.Equals(0))
                    throw new Exception(String.Format("Id [{0}] do registro inválido!", Registro.Id));

                if (String.IsNullOrEmpty(Registro.Tag))
                    throw new Exception(String.Concat("Obrigatório informar a Tag!"));

                if (!GarantiaConfiguracao.DicTagsValidas.Values.Contains(Registro.Tag))
                    throw new Exception(String.Format("Tag {0} inválida!", Registro.Tag));

                _garantiaConfigService.RegistroExcluir(Registro);
                #endregion

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
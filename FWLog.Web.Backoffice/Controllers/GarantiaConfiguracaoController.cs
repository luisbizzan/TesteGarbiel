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

        #region [Genérico] Incluir
        [HttpPost]
        public ActionResult RegistroIncluir(GarantiaConfiguracaoViewModel Registro)
        {
            try
            {
                #region Validações
                Func<ViewResult> errorView = () => { return View(Registro); };

                if (!ModelState.IsValid)
                    throw new Exception(ModelState.Values.Where(x => x.Errors.Count > 0).Aggregate("", (current, s) => current + (s.Errors[0].ErrorMessage + "<br />")));

                var RegistroConvertido = ConverterRegistro(Registro);

                if (RegistroConvertido.Tag.Equals(GarantiaConfiguracao.TAG.CONFIG.ToString(), StringComparison.InvariantCulture))
                    errorView = () => { return View(RegistroConvertido.RegistroConfiguracao); };

                if (RegistroConvertido.Tag.Equals(GarantiaConfiguracao.TAG.FORN_QUEBRA.ToString(), StringComparison.InvariantCulture))
                    errorView = () => { return View(RegistroConvertido.RegistroFornecedorQuebra); };

                if (RegistroConvertido.Tag.Equals(GarantiaConfiguracao.TAG.REM_CONFIG.ToString(), StringComparison.InvariantCulture))
                    errorView = () => { return View(RegistroConvertido.RegistroRemessaConfiguracao); };

                if (RegistroConvertido.Tag.Equals(GarantiaConfiguracao.TAG.REM_USUARIO.ToString(), StringComparison.InvariantCulture))
                    errorView = () => { return View(RegistroConvertido.RegistroRemessaUsuario); };

                if (RegistroConvertido.Tag.Equals(GarantiaConfiguracao.TAG.SANKHYA_TOP.ToString(), StringComparison.InvariantCulture))
                    errorView = () => { return View(RegistroConvertido.RegistroSankhyaTop); };

                if (!ModelState.IsValid)
                    throw new Exception(ModelState.Values.Where(x => x.Errors.Count > 0).Aggregate("", (current, s) => current + (s.Errors[0].ErrorMessage + "<br />")));
                #endregion

                _garantiaConfigService.RegistroIncluir(RegistroConvertido);

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

        #region [Genérico] Converter TAG
        private static GarantiaConfiguracao ConverterRegistro(GarantiaConfiguracaoViewModel model)
        {
            try
            {
                var _Retorno = new GarantiaConfiguracao() { Tag = model.Tag };

                #region Processamento 
                switch (model.Tag)
                {
                    case "CONFIG":
                        _Retorno.RegistroConfiguracao = new List<GarantiaConfiguracao.Configuracao>();
                        model.Inclusao.ForEach(delegate (object o)
                        {
                            _Retorno.RegistroConfiguracao.Add(GarantiaConfiguracao.SerializarJS.Deserialize<GarantiaConfiguracao.Configuracao>(o.ToString()));
                        });
                        break;
                    case "FORN_QUEBRA":
                        _Retorno.RegistroFornecedorQuebra = new List<GarantiaConfiguracao.FornecedorQuebra>();
                        model.Inclusao.ForEach(delegate (object o)
                        {
                            _Retorno.RegistroFornecedorQuebra.Add(GarantiaConfiguracao.SerializarJS.Deserialize<GarantiaConfiguracao.FornecedorQuebra>(o.ToString()));
                        });
                        break;
                    case "REM_CONFIG":
                        _Retorno.RegistroRemessaConfiguracao = new List<GarantiaConfiguracao.RemessaConfiguracao>();
                        model.Inclusao.ForEach(delegate (object o)
                        {
                            _Retorno.RegistroRemessaConfiguracao.Add(GarantiaConfiguracao.SerializarJS.Deserialize<GarantiaConfiguracao.RemessaConfiguracao>(o.ToString()));
                        });
                        break;
                    case "SANKHYA_TOP":
                        _Retorno.RegistroSankhyaTop = new List<GarantiaConfiguracao.SankhyaTop>();
                        model.Inclusao.ForEach(delegate (object o)
                        {
                            _Retorno.RegistroSankhyaTop.Add(GarantiaConfiguracao.SerializarJS.Deserialize<GarantiaConfiguracao.SankhyaTop>(o.ToString()));
                        });
                        break;
                    case "REM_USUARIO":
                        _Retorno.RegistroRemessaUsuario = new List<GarantiaConfiguracao.RemessaUsuario>();
                        model.Inclusao.ForEach(delegate (object o)
                        {
                            _Retorno.RegistroRemessaUsuario.Add(GarantiaConfiguracao.SerializarJS.Deserialize<GarantiaConfiguracao.RemessaUsuario>(o.ToString()));
                        });
                        break;
                    default:
                        throw new Exception(String.Format("[ConverterRegistro] A Tag {0} é inválida!", model.Tag));
                }
                #endregion

                return _Retorno;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
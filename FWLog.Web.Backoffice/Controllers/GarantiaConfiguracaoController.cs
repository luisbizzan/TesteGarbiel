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

                if (RegistroConvertido.Tag.Equals(GarantiaConfiguracao.GarantiaTag.Configuracao))
                {
                    RegistroConvertido.RegistroConfiguracao.ForEach(delegate (GarantiaConfiguracao.Configuracao config)
                    {
                        if (config.Id_Filial_Sankhya.Equals(0)) throw new Exception("Código de Filial inválido!");
                        if (String.IsNullOrEmpty(config.Filial)) throw new Exception("Filial inválida!");
                        if (config.Pct_Estorno_Frete.Equals(0)) throw new Exception("Percentual Estorno Frete inválido!");
                        if (config.Pct_Desvalorizacao.Equals(0)) throw new Exception("Percentual Desvalorização inválido!");
                        if (config.Vlr_Minimo_Envio.Equals(0)) throw new Exception("Valor Mínimo de Envio não pode ser zero!");

                        config.Filial = config.Filial.Split(']').Length.Equals(2) ? config.Filial.Split(']')[0].Replace("[", String.Empty).Trim() : config.Filial;
                    });
                    errorView = () => { return View(RegistroConvertido.RegistroConfiguracao); };
                }

                if (RegistroConvertido.Tag.Equals(GarantiaConfiguracao.GarantiaTag.FornecedorQuebra))
                    errorView = () => { return View(RegistroConvertido.RegistroFornecedorQuebra); };

                if (RegistroConvertido.Tag.Equals(GarantiaConfiguracao.GarantiaTag.RemessaConfiguracao))
                {
                    RegistroConvertido.RegistroRemessaConfiguracao.ForEach(delegate (GarantiaConfiguracao.RemessaConfiguracao remessaConfig)
                    {
                        if (remessaConfig.Id_Filial_Sankhya.Equals(0)) throw new Exception("Código Filial inválido!");
                        if (String.IsNullOrEmpty(remessaConfig.Filial)) throw new Exception("Filial inválida!");
                        if (String.IsNullOrEmpty(remessaConfig.Cod_Fornecedor)) throw new Exception("Informe o Fornecedor!");
                        if (remessaConfig.Vlr_Minimo.Equals(0)) throw new Exception("Valor Minímo deve ser maior que zero!");
                        if (remessaConfig.Total.Equals(0)) throw new Exception("Valor Total deve ser maior que zero!");

                        remessaConfig.Filial = remessaConfig.Filial.Split(']').Length.Equals(2) ? remessaConfig.Filial.Split(']')[0].Replace("[", String.Empty).Trim() : remessaConfig.Filial;
                    });
                    errorView = () => { return View(RegistroConvertido.RegistroRemessaConfiguracao); };
                }

                if (RegistroConvertido.Tag.Equals(GarantiaConfiguracao.GarantiaTag.RemessaUsuario))
                    errorView = () => { return View(RegistroConvertido.RegistroRemessaUsuario); };

                if (RegistroConvertido.Tag.Equals(GarantiaConfiguracao.GarantiaTag.SankhyaTop))
                    errorView = () => { return View(RegistroConvertido.RegistroSankhyaTop); };

                if (RegistroConvertido.Tag.Equals(GarantiaConfiguracao.GarantiaTag.FornecedorGrupo))
                    errorView = () => { return View(RegistroConvertido.RegistroFornecedorGrupo); };

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
        public JsonResult RegistroListar(GarantiaConfiguracao.GarantiaTag TAG)
        {
            try
            {
                #region Validações
                if (!Enum.IsDefined(typeof(GarantiaConfiguracao.GarantiaTag), TAG))
                    throw new Exception(String.Format("Tag {0} inválida!", TAG.ToString()));
                #endregion

                #region [Processamento] Formatar Valores para View 
                var garantia = _garantiaConfigService.RegistroListar(TAG);

                GarantiaConfiguracao.GridNome = GarantiaConfiguracao.DicTagGridNome.Where(w => w.Key.Equals(TAG)).FirstOrDefault().Value;
                GarantiaConfiguracao.GridColunas = (object[])GarantiaConfiguracao.DicTagGridColuna.Where(w => w.Key.Equals(TAG)).FirstOrDefault().Value;

                var _Data = new Object();
                if (garantia.Tag.Equals(GarantiaConfiguracao.GarantiaTag.Configuracao))
                    _Data = garantia.RegistroConfiguracao;
                if (garantia.Tag.Equals(GarantiaConfiguracao.GarantiaTag.FornecedorQuebra))
                    _Data = garantia.RegistroFornecedorQuebra;
                if (garantia.Tag.Equals(GarantiaConfiguracao.GarantiaTag.RemessaConfiguracao))
                    _Data = garantia.RegistroRemessaConfiguracao;
                if (garantia.Tag.Equals(GarantiaConfiguracao.GarantiaTag.RemessaUsuario))
                    _Data = garantia.RegistroRemessaUsuario;
                if (garantia.Tag.Equals(GarantiaConfiguracao.GarantiaTag.SankhyaTop))
                    _Data = garantia.RegistroSankhyaTop;
                if (garantia.Tag.Equals(GarantiaConfiguracao.GarantiaTag.FornecedorGrupo))
                    _Data = garantia.RegistroFornecedorGrupo;
                #endregion

                return Json(new
                {
                    Success = true,
                    GridColunas = GarantiaConfiguracao.GridColunas.ToArray(),
                    GridNome = GarantiaConfiguracao.GridNome,
                    Data = _Data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
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

                if (!GarantiaConfiguracao.DicTagsValidas.Values.Contains(Registro.Tag.ToString()))
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

        #region [Genérico] Converter TAG
        private static GarantiaConfiguracao ConverterRegistro(GarantiaConfiguracaoViewModel model)
        {
            try
            {
                var _Retorno = new GarantiaConfiguracao() { Tag = (GarantiaConfiguracao.GarantiaTag)Enum.Parse(typeof(GarantiaConfiguracao.GarantiaTag), model.Tag) };

                #region Processamento 
                switch (_Retorno.Tag)
                {
                    #region Configuração
                    case GarantiaConfiguracao.GarantiaTag.Configuracao:
                        _Retorno.RegistroConfiguracao = new List<GarantiaConfiguracao.Configuracao>();
                        model.Inclusao.ForEach(delegate (object o)
                        {
                            _Retorno.RegistroConfiguracao.Add(GarantiaConfiguracao.SerializarJS.Deserialize<GarantiaConfiguracao.Configuracao>(o.ToString()));
                        });
                        break;
                    #endregion

                    #region Fornecedor Quebra
                    case GarantiaConfiguracao.GarantiaTag.FornecedorQuebra:
                        _Retorno.RegistroFornecedorQuebra = new List<GarantiaConfiguracao.FornecedorQuebra>();
                        model.Inclusao.ForEach(delegate (object o)
                        {
                            _Retorno.RegistroFornecedorQuebra.Add(GarantiaConfiguracao.SerializarJS.Deserialize<GarantiaConfiguracao.FornecedorQuebra>(o.ToString()));
                        });
                        break;
                    #endregion

                    #region Remessa Configuração
                    case GarantiaConfiguracao.GarantiaTag.RemessaConfiguracao:
                        _Retorno.RegistroRemessaConfiguracao = new List<GarantiaConfiguracao.RemessaConfiguracao>();
                        model.Inclusao.ForEach(delegate (object o)
                        {
                            _Retorno.RegistroRemessaConfiguracao.Add(GarantiaConfiguracao.SerializarJS.Deserialize<GarantiaConfiguracao.RemessaConfiguracao>(o.ToString()));
                        });
                        break;
                    #endregion

                    #region Sankhya Top
                    case GarantiaConfiguracao.GarantiaTag.SankhyaTop:
                        _Retorno.RegistroSankhyaTop = new List<GarantiaConfiguracao.SankhyaTop>();
                        model.Inclusao.ForEach(delegate (object o)
                        {
                            _Retorno.RegistroSankhyaTop.Add(GarantiaConfiguracao.SerializarJS.Deserialize<GarantiaConfiguracao.SankhyaTop>(o.ToString()));
                        });
                        break;
                    #endregion

                    #region Remessa Usuário
                    case GarantiaConfiguracao.GarantiaTag.RemessaUsuario:
                        _Retorno.RegistroRemessaUsuario = new List<GarantiaConfiguracao.RemessaUsuario>();
                        model.Inclusao.ForEach(delegate (object o)
                        {
                            _Retorno.RegistroRemessaUsuario.Add(GarantiaConfiguracao.SerializarJS.Deserialize<GarantiaConfiguracao.RemessaUsuario>(o.ToString()));
                        });
                        break;
                    #endregion

                    #region Fornecedor Grupo
                    case GarantiaConfiguracao.GarantiaTag.FornecedorGrupo:
                        _Retorno.RegistroFornecedorGrupo = new List<GarantiaConfiguracao.FornecedorGrupo>();
                        model.Inclusao.ForEach(delegate (object o)
                        {
                            _Retorno.RegistroFornecedorGrupo.Add(GarantiaConfiguracao.SerializarJS.Deserialize<GarantiaConfiguracao.FornecedorGrupo>(o.ToString()));
                        });
                        break;
                    #endregion

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

        #region [Genérico] AutoComplete
        [HttpPost]
        public JsonResult AutoComplete(GarantiaConfiguracao.AutoComplete autocomplete)
        {
            try
            {
                #region Erros 
                Func<ViewResult> errorView = () => { return View(autocomplete); };
                if (!ModelState.IsValid)
                    throw new Exception(ModelState.Values.Where(x => x.Errors.Count > 0).Aggregate("", (current, s) => current + (s.Errors[0].ErrorMessage + "<br />")));
                #endregion

                var _lista = _garantiaConfigService.AutoComplete(autocomplete);

                return Json(new
                {
                    Success = true,
                    Lista = _lista
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region [Sankhya Tops] Listar Ids Negociação
        [HttpGet]
        public JsonResult ListarIdNegociacao()
        {
            try
            {
                #region [Processamento] Formatar Valores para View 
                var garantiaConfiguracao = _garantiaConfigService.ListarIdNegociacao();
                #endregion

                return Json(new
                {
                    Success = true,
                    Lista = garantiaConfiguracao.ListaAutoComplete
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}
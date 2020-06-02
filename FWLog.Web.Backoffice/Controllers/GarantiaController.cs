using AutoMapper;
using FWLog.Data;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Services.Services;
using System.Collections.Generic;
using System.Web.Mvc;
using FWLog.Web.Backoffice.Helpers;
using FWLog.AspNet.Identity;
using FWLog.Web.Backoffice.Models.GarantiaCtx;
using FWLog.Web.Backoffice.Models.CommonCtx;
using System;
using System.Linq;
using ExtensionMethods.String;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using FWLog.Data.EnumsAndConsts;
using System.Net;
using Newtonsoft.Json;

namespace FWLog.Web.Backoffice.Controllers
{
    public class GarantiaController : BOBaseController
    {
        private readonly UnitOfWork _uow;
        private readonly GarantiaService _garantiaService;
        private readonly ApplicationLogService _applicationLogService;

        public GarantiaController(
            UnitOfWork uow,
            GarantiaService garantiaService,
            ApplicationLogService applicationLogService)
        {
            _uow = uow;
            _garantiaService = garantiaService;
            _applicationLogService = applicationLogService;
        }

        public ActionResult Index()
        {
            var model = new GarantiaSolicitacaoVM
            {
                Filter = new GarantiaSolicitacaoFilterVM()
                {
                    Lista_Status = new SelectList(
                  _uow.GeralRepository.TodosTiposDaCategoria("GAR_SOLICITACAO", "ID_STATUS").OrderBy(o => o.Id).Select(x => new SelectListItem
                  {
                      Value = x.Id.ToString(),
                      Text = x.Descricao,
                  }), "Value", "Text"),
                    Lista_Tipos = new SelectList(
                  _uow.GeralRepository.TodosTiposDaCategoria("GAR_SOLICITACAO", "ID_TIPO").OrderBy(o => o.Id).Select(x => new SelectListItem
                  {
                      Value = x.Id.ToString(),
                      Text = x.Descricao,
                  }), "Value", "Text")
                }
            };

            model.Filter.Data_Inicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00).AddDays(-7);
            model.Filter.Data_Final = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00).AddDays(10);
            model.Filter.Id_Empresa = IdEmpresa;

            return View(model);
        }

        public ActionResult SolicitacaoListar(DataTableFilter<GarantiaSolicitacaoFilterVM> model)
        {
            int recordsFiltered, totalRecords;
            var filter = Mapper.Map<DataTableFilter<GarantiaSolicitacaoFilter>>(model);

            IEnumerable<GarSolicitacao> result = _uow.GarantiaRepository.ListarSolicitacao(filter, out recordsFiltered, out totalRecords);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = Mapper.Map<IEnumerable<GarantiaSolicitacaoListVM>>(result)
            });
        }

        public ActionResult SolicitacaoVisualizar(long Id)
        {
            var itens = _uow.GarantiaRepository.ListarSolicitacaoItem(Id);

            var model = new GarantiaSolicitacaoItemVM
            {
                Solicitacao = Mapper.Map<GarantiaSolicitacaoListVM>(_uow.GarantiaRepository.SelecionaSolicitacao(Id)),
                Itens = Mapper.Map<List<GarantiaSolicitacaoItemListVM>>(itens)
            };

            return PartialView("_SolicitacaoVisualizar", model);
        }

        public ActionResult ConferenciaConferir(long Id, string Tipo)
        {
            //Tipo = Remessa | Solicitação
            var idConferencia = _uow.GarantiaRepository.PegaIdUltimaConferenciaAtiva(Tipo, Id);
            ViewBag.Tipo = Tipo;

            var model = new GarantiaConferenciaVM
            {
                Conferencia = Mapper.Map<GarantiaConferencia>(_uow.GarantiaRepository.SelecionaConferencia(idConferencia))
            };

            if (Tipo == "solicitacao")
            {
                model.Solicitacao = Mapper.Map<GarantiaSolicitacaoListVM>(_uow.GarantiaRepository.SelecionaSolicitacao(Id));
                return PartialView("_SolicitacaoConferir", model);
            }
            else
            {
                model.Remessa = Mapper.Map<GarantiaRemessaListVM>(_uow.GarantiaRepository.SelecionaRemessa(Id));
                return PartialView("_RemessaConferir", model);
            }
        }

        public ActionResult SolicitacaoImportar()
        {
            var model = new GarantiaSolicitacao
            {
            };

            return PartialView("_SolicitacaoImportar", model);
        }

        [HttpPost]
        public ActionResult SolicitacaoImportarGravar(GarantiaSolicitacao item)
        {
            if (item.Id_Tipo == 30 && string.IsNullOrEmpty(item.Chave_Acesso))
                ModelState.AddModelError("Chave_Acesso", "O campo Chave de Acesso é obrigatório.");

            if (item.Id_Tipo == 31 && string.IsNullOrEmpty(item.Cnpj))
                ModelState.AddModelError("Cnpj", "O campo Cnpj é obrigatório.");

            if (item.Id_Tipo == 31 && item.Numero == null)
                ModelState.AddModelError("Numero", "O campo Número é obrigatório.");

            if (item.Id_Tipo == 31 && string.IsNullOrEmpty(item.Serie))
                ModelState.AddModelError("Serie", "O campo Série é obrigatório.");

            if ((item.Id_Tipo == 32 || item.Id_Tipo == 21) && item.Numero_Interno == null)
                ModelState.AddModelError("Numero_Interno", "O campo Número Interno é obrigatório.");

            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values.Where(x => x.Errors.Count > 0)
                    .Aggregate("", (current, s) => current + (s.Errors[0].ErrorMessage + "<br />"));
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = erros
                });
            }
            else
            {
                item.Codigo_Postagem = string.IsNullOrEmpty(item.Codigo_Postagem) ? "REPRESENTANTE" : item.Codigo_Postagem;

                var retorno = new DmlStatus();

                if (item.Id_Tipo == 31 || item.Id_Tipo == 32)
                {
                    //IMPORTA SOLICITAÇÃO SE FOR CARTA OU NOTA MANUAL
                    retorno = _uow.GarantiaRepository.ImportarSolicitacaoNfCartaManual(new GarSolicitacao
                    {
                        Cli_Cnpj = item.Cnpj,
                        Nota_Fiscal = item.Numero_Interno == null ? item.Numero.ToString() : item.Numero_Interno.ToString(),
                        Id_Usr = IdUsuario,
                        Id_Empresa = IdEmpresa,
                        Serie = item.Serie,
                        Codigo_Postagem = item.Codigo_Postagem,
                        Id_Tipo_Doc = item.Id_Tipo
                    });
                }
                else if (item.Id_Tipo == 30 || item.Id_Tipo == 21)
                {
                    //IMPORTA SOLICITAÇÃO SE FOR PEDIDO OU NOTA ELETRONICA
                    retorno = _uow.GarantiaRepository.ImportarSolicitacaoNfEletronicaPedido(new GarSolicitacao
                    {
                        Id_Sav = item.Numero_Interno ?? 0,
                        Id_Usr = IdUsuario,
                        Id_Empresa = IdEmpresa,
                        Id_Tipo_Doc = item.Id_Tipo,
                        Codigo_Postagem = item.Codigo_Postagem
                    });
                }

                if (retorno.Id != 0)
                {
                    //CRIAR A CONFERENCIA DE ENTRADA
                    _uow.GarantiaRepository.CriarConferenciaEntrada(new GarConferencia
                    {
                        Id_Solicitacao = retorno.Id,
                        Id_Usr = IdUsuario,
                        Id_Tipo_Conf = 5
                    });
                }

                if (retorno.Sucesso)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = true,
                        Message = Resources.CommonStrings.RegisterCreatedSuccessMessage
                    });
                }
                else
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = retorno.Mensagem
                    });
                }
            }
        }

        [HttpPost]
        public ActionResult SolicitacaoEstornar(long Id)
        {
            _uow.GarantiaRepository.EstornarSolicitacao(new GarSolicitacao
            {
                Id = Id
            });

            return Json(new AjaxGenericResultModel
            {
                Success = true,
                Message = "Estorno efetuado com sucesso."
            });
        }

        public ActionResult ConferenciaForm(long Id_Conferencia)
        {
            var conferencia = _uow.GarantiaRepository.SelecionaConferencia(Id_Conferencia);
            var model = new GarantiaConferenciaFormVM
            {
                Form = new GarantiaConferenciaItem
                {
                    Id = Id_Conferencia,
                    Id_Tipo_Conf = conferencia.Id_Tipo_Conf,
                }
            };

            return PartialView("_ConferenciaForm", model);
        }

        [HttpPost]
        public ActionResult AtualizarItemConferencia(GarantiaConferenciaItem item)
        {
            var conferencia = _uow.GarantiaRepository.SelecionaConferencia(item.Id_Conf);

            if (string.IsNullOrEmpty(item.Refx))
                ModelState.AddModelError("Refx", "O campo Código é obrigatório.");

            if (item.Quant_Conferida == 0 || item.Quant_Conferida == null)
                ModelState.AddModelError("Quant_Conferida", "O campo Quantidade é obrigatório.");

            if ((conferencia.Id_Tipo_Conf == 26 || conferencia.Id_Tipo_Conf == 6) && item.Id_Solicitacao == null)
                ModelState.AddModelError("Id_Solicitacao", "O campo Solicitação é obrigatório.");

            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values.Where(x => x.Errors.Count > 0)
                    .Aggregate("", (current, s) => current + (s.Errors[0].ErrorMessage + "<br />"));
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = erros
                });
            }
            else
            {
                //VERIFICA SE ITEM EXISTE
                if (!_uow.GarantiaRepository.ItemExiste(item.Refx))
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = string.Format("Item {0} não existe!", item.Refx)
                    });

                //ATUALIZA A QUANTIDADE CONFERIDA
                if (conferencia.Id_Tipo_Conf == 26 || conferencia.Id_Tipo_Conf == 6)
                {
                    //Remessa - Envio Fornecedor ou Retorno Fornecedor
                    _uow.GarantiaRepository.AtualizarItemConferenciaRemessa(new GarConferenciaItem
                    {
                        Quant_Conferida = item.Quant_Conferida,
                        Refx = item.Refx,
                        Id_Usr = IdUsuario,
                        Id_Solicitacao = item.Id_Solicitacao,
                        Id_Conf = item.Id_Conf
                    });
                }
                else
                {
                    _uow.GarantiaRepository.AtualizarItemConferencia(new GarConferenciaItem
                    {
                        Quant_Conferida = item.Quant_Conferida,
                        Refx = item.Refx,
                        Id_Usr = IdUsuario,
                        Id_Conf = item.Id_Conf
                    });
                }

                //GRAVA O HISTORICO
                _uow.GarantiaRepository.InserirConferenciaHistorico(new GarConferenciaHist
                {
                    Quant_Conferida = item.Quant_Conferida,
                    Refx = item.Refx,
                    Id_Solicitacao = item.Id_Solicitacao,
                    Volume = "",
                    Id_Usr = IdUsuario,
                    Id_Conf = item.Id_Conf
                });

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = string.Format("{0} - {1} Processado!", item.Refx, item.Quant_Conferida)
                });
            }
        }

        public ActionResult ConferenciaItemPendente(long Id_Conferencia)
        {
            var conferencia = _uow.GarantiaRepository.SelecionaConferencia(Id_Conferencia);
            ViewBag.Id_Tipo_Conf = conferencia.Id_Tipo_Conf;

            var result = _uow.GarantiaRepository.ListarConferenciaItemPendente(Id_Conferencia);
            var model = Mapper.Map<IEnumerable<GarantiaConferenciaItem>>(result).ToList();

            return PartialView("_ConferenciaItemPendente", model);
        }

        public ActionResult ConferenciaItemConferido(long Id_Conferencia)
        {
            var conferencia = _uow.GarantiaRepository.SelecionaConferencia(Id_Conferencia);
            ViewBag.Id_Tipo_Conf = conferencia.Id_Tipo_Conf;

            var result = _uow.GarantiaRepository.ListarConferenciaHistorico(Id_Conferencia);
            var model = Mapper.Map<IEnumerable<GarantiaConferenciaItem>>(result).ToList();

            return PartialView("_ConferenciaItemConferido", model);
        }

        public ActionResult ConferenciaItemConferidoRemessa(long Id_Remessa)
        {
            var conferencia = _uow.GarantiaRepository.SelecionaConferenciaDaRemessa(Id_Remessa);
            ViewBag.Id_Tipo_Conf = conferencia.Id_Tipo_Conf;

            var result = _uow.GarantiaRepository.ListarConferenciaHistorico(conferencia.Id);
            var model = Mapper.Map<IEnumerable<GarantiaConferenciaItem>>(result).ToList();

            return PartialView("_ConferenciaItemConferido", model);
        }

        public ActionResult ConferenciaConferirManual(long Id_Conferencia)
        {
            var model = new GarantiaConferenciaFormVM
            {
                Form = new GarantiaConferenciaItem
                {
                    Id_Conf = Id_Conferencia
                },
                Lista_Refx = new SelectList(_uow.GarantiaRepository.ListarRemessaRefx(Id_Conferencia), "Refx", "Refx", 0)
            };

            return PartialView("_ConferenciaConferirManual", model);
        }

        public ActionResult ConferenciaListarRemessaSolicitacaoAjax(string Refx, long Id_Conferencia)
        {
            try
            {
                var dados = _uow.GarantiaRepository.ListarRemessaSolicitacao(Id_Conferencia, Refx);
                return Json(dados, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { success = false, errors = "Falha ao listar" });
            }
        }

        public ActionResult ConferenciaLaudo(long Id_Conferencia)
        {
            var conferencia = _uow.GarantiaRepository.SelecionaConferencia(Id_Conferencia);

            var result = _uow.GarantiaRepository.ListarConferenciaSolicitacaoLaudo(Id_Conferencia);
            var lista = Mapper.Map<IEnumerable<GarantiaLaudo>>(result).ToList();

            var model = new GarantiaLaudoVM
            {
                Conferencia = new GarantiaConferenciaItem { Id_Tipo_Conf = conferencia.Id_Tipo_Conf },
                Lista = lista
            };

            return PartialView("_ConferenciaLaudo", model);
        }

        public ActionResult ConferenciaLaudoDetalhe(long Id_Conferencia, string Refx)
        {
            var conferencia = _uow.GarantiaRepository.SelecionaConferencia(Id_Conferencia);
            var conferenciaItem = _uow.GarantiaRepository.SelecionaConferenciaItem(Id_Conferencia, Refx);

            var result = _uow.GarantiaRepository.ListarConferenciaSolicitacaoLaudoDetalhe(Id_Conferencia, Refx);
            var lista = Mapper.Map<IEnumerable<GarantiaLaudo>>(result).ToList();

            //Tipo Solicitação => 17 = Devolução | 18 = Garantia
            //Tipo Laudo => 9 = Sinistro | 8 = Defeito
            var idTipoLaudo = conferencia.Id_Tipo_Solicitacao == 17 ? 9 : 8;

            var model = new GarantiaLaudoVM
            {
                Conferencia = Mapper.Map<GarantiaConferenciaItem>(conferenciaItem),
                Form = new GarantiaLaudo()
                {
                    Refx = Refx,
                    Lista_Motivos = new SelectList(_uow.GarantiaRepository.ListarMotivoLaudo(idTipoLaudo), "Id", "Descricao", 1)
                },
                Lista = lista
            };

            return PartialView("_ConferenciaLaudoDetalhe", model);
        }

        [HttpPost]
        public ActionResult ConferenciaLaudoDetalheGravar(GarantiaLaudo item)
        {
            if (item.Quant > item.Quant_Max)
                ModelState.AddModelError("Quant", string.Format("Quantidade máxima permitida é <strong>{0}</strong>.", item.Quant_Max));

            if (item.Quant <= 0)
                ModelState.AddModelError("Quant", string.Format("Quantidade minima permitida é <strong>1</strong>."));

            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values.Where(x => x.Errors.Count > 0)
                    .Aggregate("", (current, s) => current + (s.Errors[0].ErrorMessage + "<br />"));
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = erros
                });
            }
            else
            {
                //todo fazer verificações de quantidade
                _uow.GarantiaRepository.CriarLaudo(new GarSolicitacaoItemLaudo
                {
                    Id_Item = item.Id_Item,
                    Id_Motivo = item.Id_Motivo,
                    Id_Tipo_Retorno = 4,
                    Quant = item.Quant
                });

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = Resources.CommonStrings.RegisterCreatedSuccessMessage
                });
            }
        }

        [HttpPost]
        public ActionResult ConferenciaLaudoDetalheExcluir(long Id)
        {
            //todo fazer verificações de quantidade
            _uow.GarantiaRepository.ExcluirLaudo(Id);

            return Json(new AjaxGenericResultModel
            {
                Success = true,
                Message = Resources.CommonStrings.RegisterDeletedSuccessMessage
            });
        }

        public ActionResult ConferenciaDivergencia(long Id_Conferencia)
        {
            var conferencia = _uow.GarantiaRepository.SelecionaConferencia(Id_Conferencia);

            var result = _uow.GarantiaRepository.ListarConferenciaItem(Id_Conferencia);
            var model = new GarantiaConferenciaDivergenciaVM
            {
                Cabecalho = new GarantiaConferenciaItem { Id_Tipo_Conf = conferencia.Id_Tipo_Conf },
                Itens = Mapper.Map<IEnumerable<GarantiaConferenciaItem>>(result).ToList()
            };

            return PartialView("_ConferenciaDivergencia", model);
        }

        public ActionResult Teste(long Id)
        {
            //GRAVA O HISTORICO
            _uow.GarantiaRepository.CriarConferenciaEntrada(new GarConferencia
            {
                Id_Solicitacao = Id,
                Id_Usr = IdUsuario,
                Id_Tipo_Conf = 5
            });

            return Json(new AjaxGenericResultModel
            {
                Success = true,
                Message = Resources.CommonStrings.RegisterCreatedSuccessMessage
            });
        }

        [HttpPost]
        public ActionResult ConferenciaFinalizar(long Id_Conferencia)
        {
            var conferencia = _uow.GarantiaRepository.SelecionaConferencia(Id_Conferencia);

            if (conferencia.Id_Remessa == 0)
            {
                //SOLICITAÇÃO
                //Tipo Solicitação => 17 = Devolução | 18 = Garantia
                if (conferencia.Id_Tipo_Solicitacao == 18)
                {
                    //GARANTIA
                    if (conferencia.Id_Tipo_Conf == 5)
                    {
                        //CONFERENCIA DE ENTRADA
                        _uow.GarantiaRepository.FinalizarConferenciaEntrada(new GarConferencia
                        {
                            Id = conferencia.Id,
                            Id_Usr = IdUsuario,
                            Id_Tipo_Solicitacao = conferencia.Id_Tipo_Solicitacao,
                            Id_Solicitacao = conferencia.Id_Solicitacao
                        });

                        //TOOD PARTE DE NF SANKYA
                    }
                }
                else if (conferencia.Id_Tipo_Solicitacao == 17)
                {
                    //DEVOLUÇÃO
                    if (conferencia.Id_Tipo_Conf == 5)
                    {
                        //CONFERENCIA DE ENTRADA
                        _uow.GarantiaRepository.FinalizarConferenciaEntrada(new GarConferencia
                        {
                            Id = conferencia.Id,
                            Id_Usr = IdUsuario,
                            Id_Tipo_Solicitacao = conferencia.Id_Tipo_Solicitacao,
                            Id_Solicitacao = conferencia.Id_Solicitacao
                        });

                        //TOOD PARTE DE NF SANKYA
                    }
                }
            }
            else
            {
                //REMESSA

                if (conferencia.Id_Tipo_Conf == 26)
                {
                    //CONFERENCIA DE REMESSA ENVIO FORNECEDOR

                    //TODO MANDAR NOTA PRO SANYKA E MUDAR O STATUS PARA AGUARDANDO NF
                    _uow.GarantiaRepository.AtualizaStatusConferenciaRemessa(conferencia.Id_Remessa);

                    //SE VEIO DE UMA REMESSA AUTOMATICA, FINALIZA A LISTA
                    _uow.GarantiaRepository.FinalizarRemessaAutomatica(conferencia.Id_Remessa);

                    //TODO DEPOIS ESSA ROTINA SÓ VAI SER CHAMADO QDO TIVER RETORNO DA NOTA DO SANKYA
                    //_uow.GarantiaRepository.FinalizarConferenciaRemessa(new GarConferencia
                    //{
                    //    Id = conferencia.Id,
                    //    Id_Empresa = IdEmpresa,
                    //    Id_Usr = IdUsuario,
                    //    Id_Remessa = conferencia.Id_Remessa
                    //});

                    //todo finalizar remessa lista
                }
            }

            return Json(new AjaxGenericResultModel
            {
                Success = true,
                Message = Resources.CommonStrings.RegisterCreatedSuccessMessage
            });
        }

        [HttpPost]
        public ActionResult ConferenciaVerificacaoFinalizar(long Id_Conferencia)
        {
            var conferencia = _uow.GarantiaRepository.SelecionaConferencia(Id_Conferencia);

            //Remessa - Envio Fornecedor ou Retorno Fornecedor
            if (conferencia.Id_Tipo_Conf == 26 || conferencia.Id_Tipo_Conf == 6)
            {
                var retorno = _uow.GarantiaRepository.VerificarConferenciaRemessa(conferencia);

                if (!retorno.Sucesso)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = retorno.Mensagem
                    });
                }
            }

            return Json(new AjaxGenericResultModel
            {
                Success = true,
                Message = Resources.CommonStrings.RegisterCreatedSuccessMessage
            });
        }

        public ActionResult Remessa()
        {
            var model = new GarantiaRemessaVM
            {
                Filter = new GarantiaRemessaFilterVM()
                {
                    Lista_Status = new SelectList(
                  _uow.GeralRepository.TodosTiposDaCategoria("GAR_SOLICITACAO", "ID_STATUS").OrderBy(o => o.Id).Select(x => new SelectListItem
                  {
                      Value = x.Id.ToString(),
                      Text = x.Descricao,
                  }), "Value", "Text")
                }
            };

            model.Filter.Data_Inicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00).AddDays(-7);
            model.Filter.Data_Final = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00).AddDays(10);
            model.Filter.Id_Empresa = IdEmpresa;

            //PROCESSA A FILA DE REMESSA AUTOMATICA PRO USUARIO
            _uow.GarantiaRepository.ProcessarUsrRemessaAutomatica(new GarRemessaLista
            {
                Id_Usr = IdUsuario,
                Id_Empresa = IdEmpresa
            });

            return View(model);
        }

        public ActionResult ListarRemessa(DataTableFilter<GarantiaRemessaFilterVM> model)
        {
            model.CustomFilter.Id_User = IdUsuario;
            int recordsFiltered, totalRecords;
            var filter = Mapper.Map<DataTableFilter<GarantiaRemessaFilter>>(model);

            IEnumerable<GarRemessa> result = _uow.GarantiaRepository.ListarRemessa(filter, out recordsFiltered, out totalRecords);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = Mapper.Map<IEnumerable<GarantiaRemessaListVM>>(result)
            });
        }

        public ActionResult RemessaCriar()
        {
            var model = new GarantiaRemessa
            {
            };

            return PartialView("_RemessaCriar", model);
        }

        [HttpPost]
        public ActionResult RemessaCriarGravar(GarantiaRemessa item)
        {
            var retorno = new DmlStatus();

            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values.Where(x => x.Errors.Count > 0)
                    .Aggregate("", (current, s) => current + (s.Errors[0].ErrorMessage + "<br />"));
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = erros
                });
            }
            else
            {
                try
                {
                    retorno = _uow.GarantiaRepository.CriarRemessa(new GarRemessa
                    {
                        Cod_Fornecedor = item.Cod_Fornecedor,
                        Id_Empresa = IdEmpresa,
                        Id_Status = 38,
                        Id_Tipo = 2,
                        Id_Usr = IdUsuario
                    });

                    if (retorno.Id != 0)
                    {
                        //CRIAR A CONFERENCIA DE REMESSA
                        var Id_Conferencia = _uow.GarantiaRepository.CriarConferenciaRemessa(new GarConferencia
                        {
                            Id_Remessa = retorno.Id,
                            Id_Empresa = IdEmpresa,
                            Id_Usr = IdUsuario,
                            Id_Tipo_Conf = 26 //26 Envio Fornecedor ou 6 Retorno Fornecedor (perguntar)
                        });

                        if (Id_Conferencia != 0)
                        {
                            //ADICIONAR ITENS DA CONFERENCIA DE REMESSA
                            _uow.GarantiaRepository.AdicionarConferenciaRemessaItem(new GarConferencia
                            {
                                Id_Empresa = IdEmpresa,
                                Id_Usr = IdUsuario,
                                Id = Id_Conferencia
                            });
                        }
                    }

                    if (retorno.Sucesso)
                    {
                        return Json(new AjaxGenericResultModel
                        {
                            Success = true,
                            Data = retorno.Id.ToString(),
                            Message = Resources.CommonStrings.RegisterCreatedSuccessMessage
                        });
                    }
                    else
                    {
                        return Json(new AjaxGenericResultModel
                        {
                            Success = false,
                            Message = retorno.Mensagem
                        });
                    }
                }
                catch (Exception e)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = e.Message
                    });

                    throw;
                }
            }
        }

        [HttpPost]
        public ActionResult RemessaAtualizarItemGravar(long Id_Conferencia)
        {
            try
            {
                //ADICIONA ITENS DA CONFERENCIA DE REMESSA
                _uow.GarantiaRepository.AdicionarConferenciaRemessaItem(new GarConferencia
                {
                    Id_Empresa = IdEmpresa,
                    Id_Usr = IdUsuario,
                    Id = Id_Conferencia
                });

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = Resources.CommonStrings.RegisterCreatedSuccessMessage
                });
            }
            catch (Exception e)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = e.Message
                });

                throw;
            }
        }

        public ActionResult RemessaVisualizar(long Id)
        {
            var itens = _uow.GarantiaRepository.ListarRemessaItem(Id);

            var model = new GarantiaSolicitacaoItemVM
            {
                Remessa = Mapper.Map<GarantiaRemessaListVM>(_uow.GarantiaRepository.SelecionaRemessa(Id)),
                Itens = Mapper.Map<List<GarantiaSolicitacaoItemListVM>>(itens)
            };

            return PartialView("_RemessaVisualizar", model);
        }

        public ActionResult RemessaDetalhadoVisualizar(long Id)
        {
            var itens = _uow.GarantiaRepository.ListarRemessaItemDetalhado(Id);

            var model = new GarantiaSolicitacaoItemVM
            {
                Itens = Mapper.Map<List<GarantiaSolicitacaoItemListVM>>(itens)
            };

            return PartialView("_RemessaDetalhadoVisualizar", model);
        }

        [HttpPost]
        public ActionResult RemessaEstornar(long Id)
        {
            _uow.GarantiaRepository.EstornarRemessa(new GarConferencia
            {
                Id_Remessa = Id
            });

            //SE VEIO DE UMA REMESSA AUTOMATICA, FINALIZA A LISTA
            _uow.GarantiaRepository.FinalizarRemessaAutomatica(Id);

            return Json(new AjaxGenericResultModel
            {
                Success = true,
                Message = "Estorno efetuado com sucesso."
            });
        }

        [HttpPost]
        public ActionResult RemessaAutomaticaProcessarUsr()
        {
            var conferencia = _uow.GarantiaRepository.ProcessarUsrRemessaAutomatica(new GarRemessaLista
            {
                Id_Usr = IdUsuario,
                Id_Empresa = IdEmpresa
            });

            return Json(new AjaxGenericResultModel
            {
                Success = conferencia.Sucesso,
                Message = conferencia.Mensagem
            });
        }

        [HttpPost]
        public ActionResult RemessaAutomaticaCriarUsr()
        {
            var conferencia = _uow.GarantiaRepository.CriarUsrRemessaAutomatica(new GarRemessaLista
            {
                Id_Usr = IdUsuario,
                Id_Empresa = IdEmpresa
            });

            return Json(new AjaxGenericResultModel
            {
                DataObject = conferencia.DadosObjeto,
                Success = conferencia.Sucesso,
                Message = conferencia.Mensagem
            });
        }
    }
}
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

            return View(model);
        }

        public ActionResult SolicitacaoListar(DataTableFilter<GarantiaSolicitacaoFilterVM> model)
        {
            int recordsFiltered, totalRecords;
            var filter = Mapper.Map<DataTableFilter<GarantiaSolicitacaoFilter>>(model);

            var teste = IdEmpresa;

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

        public ActionResult SolicitacaoConferir(long Id)
        {
            //Origem - Remessa | Solicitação
            var idConferencia = _uow.GarantiaRepository.PegaIdUltimaConferenciaAtiva("solicitacao", Id);

            var model = new GarantiaConferenciaVM
            {
                Conferencia = Mapper.Map<GarantiaConferencia>(_uow.GarantiaRepository.SelecionaConferencia(idConferencia)),
                Solicitacao = Mapper.Map<GarantiaSolicitacaoListVM>(_uow.GarantiaRepository.SelecionaSolicitacao(Id))
            };

            return PartialView("_SolicitacaoConferir", model);
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

            if (item.Id_Tipo == 31 && string.IsNullOrEmpty(item.Numero))
                ModelState.AddModelError("Numero", "O campo Número é obrigatório.");

            if (item.Id_Tipo == 31 && string.IsNullOrEmpty(item.Serie))
                ModelState.AddModelError("Serie", "O campo Série é obrigatório.");

            if (item.Id_Tipo == 32 && string.IsNullOrEmpty(item.Numero_Interno))
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

                long idSolicitacao = 0;

                if (item.Id_Tipo == 31 || item.Id_Tipo == 32)
                {
                    //IMPORTA SOLICITAÇÃO SE FOR CARTA OU NOTA MANUAL
                    idSolicitacao = _uow.GarantiaRepository.ImportarSolicitacaoNfCartaManual(new GarSolicitacao
                    {
                        Cli_Cnpj = item.Cnpj,
                        Nota_Fiscal = item.Numero_Interno == null ? item.Numero : item.Numero_Interno,
                        Id_Usr = IdUsuario,
                        Serie = item.Serie,
                        Codigo_Postagem = item.Codigo_Postagem,
                        Id_Tipo_Doc = item.Id_Tipo
                    });
                }
                else if (item.Id_Tipo == 30 || item.Id_Tipo == 21)
                {
                    //IMPORTA SOLICITAÇÃO SE FOR PEDIDO OU NOTA ELETRONICA
                    idSolicitacao = _uow.GarantiaRepository.ImportarSolicitacaoNfEletronicaPedido(new GarSolicitacao
                    {
                        Chave_Acesso = item.Chave_Acesso,
                        Id_Usr = IdUsuario,
                        Id_Tipo_Doc = item.Id_Tipo,
                        Codigo_Postagem = item.Codigo_Postagem
                    });
                }

                if (idSolicitacao != 0)
                {
                    //CRIAR A CONFERENCIA DE ENTRADA
                    _uow.GarantiaRepository.CriarConferencia(new GarConferencia
                    {
                        Id_Solicitacao = idSolicitacao,
                        Id_Usr = IdUsuario,
                        Id_Tipo_Conf = 5
                    });
                }

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = Resources.CommonStrings.RegisterCreatedSuccessMessage
                });
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
            var model = new GarantiaConferenciaFormVM
            {
                Form = new GarantiaConferenciaItem
                {
                    Id = Id_Conferencia
                }
            };

            return PartialView("_ConferenciaForm", model);
        }

        [HttpPost]
        public ActionResult AtualizarItemConferencia(GarantiaConferenciaItem item)
        {
            if (string.IsNullOrEmpty(item.Refx))
                ModelState.AddModelError("Refx", "O campo Código é obrigatório.");

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
                //ATUALIZA A QUANTIDADE CONFERIDA
                _uow.GarantiaRepository.AtualizarItemConferencia(new GarConferenciaItem
                {
                    Quant_Conferida = item.Quant_Conferida,
                    Refx = item.Refx,
                    Id_Usr = IdUsuario,
                    Id_Conf = item.Id_Conf
                });

                //GRAVA O HISTORICO
                _uow.GarantiaRepository.InserirConferenciaHistorico(new GarConferenciaHist
                {
                    Quant_Conferida = item.Quant_Conferida,
                    Refx = item.Refx,
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
            var result = _uow.GarantiaRepository.ListarConferenciaItemPendente(Id_Conferencia);
            var model = Mapper.Map<IEnumerable<GarantiaConferenciaItem>>(result).ToList();

            return PartialView("_ConferenciaItemPendente", model);
        }

        public ActionResult ConferenciaItemConferido(long Id_Conferencia)
        {
            var result = _uow.GarantiaRepository.ListarConferenciaHistorico(Id_Conferencia);
            var model = Mapper.Map<IEnumerable<GarantiaConferenciaItem>>(result).ToList();

            return PartialView("_ConferenciaItemConferido", model);
        }

        public ActionResult ConferenciaLaudo(long Id_Conferencia)
        {
            var result = _uow.GarantiaRepository.ListarConferenciaSolicitacaoLaudo(Id_Conferencia);
            var lista = Mapper.Map<IEnumerable<GarantiaLaudo>>(result).ToList();

            var model = new GarantiaLaudoVM
            {
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
            var result = _uow.GarantiaRepository.ListarConferenciaItem(Id_Conferencia);
            var model = new GarantiaConferenciaDivergenciaVM
            {
                Itens = Mapper.Map<IEnumerable<GarantiaConferenciaItem>>(result).ToList()
            };

            return PartialView("_ConferenciaDivergencia", model);
        }

        public ActionResult Teste(long Id)
        {
            //GRAVA O HISTORICO
            _uow.GarantiaRepository.CriarConferencia(new GarConferencia
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
                        Id_Solicitacao = conferencia.Id_Solicitacao
                    });

                    //TOOD PARTE DE NF SANKYA
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

            return View(model);
        }

        public ActionResult ListarRemessa(DataTableFilter<GarantiaRemessaFilterVM> model)
        {
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
    }
}
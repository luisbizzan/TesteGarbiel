﻿using AutoMapper;
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

        public ActionResult ListarSolicitacao(DataTableFilter<GarantiaSolicitacaoFilterVM> model)
        {
            int recordsFiltered, totalRecords;
            var filter = Mapper.Map<DataTableFilter<GarantiaFilter>>(model);

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

        public ActionResult VisualizarSolicitacao(long Id)
        {
            var itens = _uow.GarantiaRepository.ListarSolicitacaoItem(Id);

            var model = new GarantiaSolicitacaoItemVM
            {
                Solicitacao = Mapper.Map<GarantiaSolicitacaoListVM>(_uow.GarantiaRepository.SelecionaSolicitacao(Id)),
                Itens = Mapper.Map<List<GarantiaSolicitacaoItemListVM>>(itens)
            };

            return PartialView("_VisualizarSolicitacao", model);
        }

        public ActionResult ConferirSolicitacao(long Id)
        {
            var itens = _uow.GarantiaRepository.ListarSolicitacaoItem(Id);

            var model = new GarantiaConferenciaVM
            {
                Solicitacao = Mapper.Map<GarantiaSolicitacaoListVM>(_uow.GarantiaRepository.SelecionaSolicitacao(Id))
            };

            return PartialView("_ConferirSolicitacao", model);
        }

        public ActionResult ImportarSolicitacao()
        {
            var model = new GarantiaSolicitacao
            {
            };

            return PartialView("_ImportarSolicitacao", model);
        }

        [HttpPost]
        public ActionResult ImportarSolicitacaoGravar(GarantiaSolicitacao item)
        {
            if (item.Id_Tipo == 1 && string.IsNullOrEmpty(item.Chave_Acesso))
                ModelState.AddModelError("Chave_Acesso", "O campo Chave de Acesso é obrigatório.");

            if (item.Id_Tipo == 2 && string.IsNullOrEmpty(item.Cnpj))
                ModelState.AddModelError("Cnpj", "O campo Cnpj é obrigatório.");

            if (item.Id_Tipo == 2 && string.IsNullOrEmpty(item.Numero))
                ModelState.AddModelError("Numero", "O campo Número é obrigatório.");

            if (item.Id_Tipo == 2 && string.IsNullOrEmpty(item.Serie))
                ModelState.AddModelError("Serie", "O campo Série é obrigatório.");

            if (item.Id_Tipo == 3 && string.IsNullOrEmpty(item.Numero_Interno))
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
                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = Resources.CommonStrings.RegisterCreatedSuccessMessage
                });
            }
        }

        public ActionResult ConferenciaForm(long Id)
        {
            var model = new GarantiaConferenciaFormVM
            {
            };

            return PartialView("_ConferenciaForm", model);
        }

        public ActionResult ConferenciaLaudo(long Id)
        {
            var lista = new List<GarantiaLaudo>();
            lista.Add(new GarantiaLaudo
            {
                Refx = "teste",
                Descricao = "aaaaa",
                Quant = 10
            });
            lista.Add(new GarantiaLaudo
            {
                Refx = "testeaa",
                Descricao = "aaaaaccc",
                Quant = 2
            });

            var model = new GarantiaLaudoVM
            {
                Lista = lista
            };

            return PartialView("_ConferenciaLaudo", model);
        }

        public ActionResult ConferenciaItensPendentes(long Id)
        {
            var model = new List<GarantiaConferencia>();
            model.Add(new GarantiaConferencia
            {
                Refx = "teste",
                Descricao = "aaaaa",
                Quantidade = 10
            });
            model.Add(new GarantiaConferencia
            {
                Refx = "testeaa",
                Descricao = "aaaaaccc",
                Quantidade = 2
            });

            return PartialView("_ConferenciaItensPendentes", model);
        }

        public ActionResult ConferenciaLaudoDetalhe(long Id)
        {
            var lista = new List<GarantiaLaudo>();
            lista.Add(new GarantiaLaudo
            {
                Motivo = "teste",
                Quant = 10
            });
            lista.Add(new GarantiaLaudo
            {
                Motivo = "testeee",
                Quant = 5
            });

            var model = new GarantiaLaudoVM
            {
                Form = new GarantiaLaudo()
                {
                    Lista_Motivos = new SelectList(_uow.GarantiaRepository.ListarMotivoLaudo(), "Id", "Descricao", "Tipo", 1)
                },
                Lista = lista
            };

            return PartialView("_ConferenciaLaudoDetalhe", model);
        }

        public ActionResult ConferenciaDivergencia(long Id)
        {
            var itens = new List<GarantiaConferenciaDivergencia>();
            itens.Add(new GarantiaConferenciaDivergencia
            {
                Descricao = "teste",
                Refx = "zzz",
                Divergencia = -5,
                Quant = 10,
                Quant_Conferida = 5
            });
            itens.Add(new GarantiaConferenciaDivergencia
            {
                Descricao = "teste",
                Refx = "zzz",
                Divergencia = 11,
                Quant = 10,
                Quant_Conferida = 11
            });

            var model = new GarantiaConferenciaDivergenciaVM
            {
                Itens = itens
            };

            return PartialView("_ConferenciaDivergencia", model);
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
            var filter = Mapper.Map<DataTableFilter<GarantiaFilter>>(model);

            IEnumerable<GarSolicitacao> result = _uow.GarantiaRepository.ListarSolicitacao(filter, out recordsFiltered, out totalRecords);

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
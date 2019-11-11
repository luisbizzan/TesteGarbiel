﻿using AutoMapper;
using FWLog.Data;
using FWLog.Data.Models.FilterCtx;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx;
using FWLog.Web.Backoffice.Models.CommonCtx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    [Route("recebimento/notas")]
    public class BORecebimentoNotaController : BOBaseController
    {
        UnitOfWork _uow;

        public BORecebimentoNotaController(UnitOfWork uow)
        {
            _uow = uow;
        }
        
        public ActionResult Index()
        {
            var model = new BORecebimentoNotaListViewModel();

            model.Filter = new BORecebimentoNotaFilterViewModel()
            {
                ListaStatus = new SelectList(
                    _uow.LoteStatusRepository.GetAll().Select(x => new SelectListItem
                    {
                        Value = x.IdLoteStatus.ToString(),
                        Text = x.Descricao
                    }), "Value", "Text"
                )
            };

            return View(model);
        }

        public ActionResult PageData(DataTableFilter<BORecebimentoNotaFilterViewModel> model)
        {
            int totalRecords = 0;
            int totalRecordsFiltered = 0;

            var lote = _uow.LoteRepository.GetAll().ToList();

            totalRecords = lote.Count;

            var query = lote.AsQueryable();

            if (!string.IsNullOrEmpty(model.CustomFilter.DANFE))
                query = query.Where(x => x.NotaFiscal.DANFE == model.CustomFilter.DANFE);

            if (!string.IsNullOrEmpty(model.CustomFilter.Lote))
                query = query.Where(x => x.IdLote == Convert.ToInt32(model.CustomFilter.Lote));

            if (model.CustomFilter.Nota != null && model.CustomFilter.Nota != 0)
                query = query.Where(x => x.NotaFiscal.Numero == model.CustomFilter.Nota);

            return DataTableResult.FromModel(new DataTableResponseModel()
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = totalRecordsFiltered,
                Data = Mapper.Map<IEnumerable<BORecebimentoNotaListItemViewModel>>(lote)
            });
        }

        public ActionResult DetalhesEtiquetaConferencia()
        {
            var viewModel = new BODetalhesEtiquetaConferenciaViewModel
            {
                NumeroNotaFiscal = "42-10/04-84.684.182/0001-57-55-001-000.000.002-010.804.210-8",
                DataHoraRecebimento = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                NomeFornecedor = "Nome do Fornecedor Nota Fiscal",
                QuantidadeVolumes = "05"
            };

            return View(viewModel);
        }
    }
}
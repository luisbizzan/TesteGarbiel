using AutoMapper;
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
            List<BORecebimentoNotaListItemViewModel> boRecebimentoNotaListItemViewModel = new List<BORecebimentoNotaListItemViewModel>();
            int totalRecords = 0;
            int totalRecordsFiltered = 0;

            var query = _uow.LoteRepository.ObterLote().AsQueryable();

            totalRecords = query.Count();

            if (!string.IsNullOrEmpty(model.CustomFilter.DANFE))
                query = query.Where(x => x.NotaFiscal.DANFE == model.CustomFilter.DANFE);

            if (model.CustomFilter.Lote != null && model.CustomFilter.Lote != 0)
                query = query.Where(x => x.IdLote == Convert.ToInt32(model.CustomFilter.Lote));

            if (model.CustomFilter.Nota != null && model.CustomFilter.Nota != 0)
                query = query.Where(x => x.NotaFiscal.Numero == model.CustomFilter.Nota);

            if (model.CustomFilter.Prazo != null && model.CustomFilter.Prazo != 0)
                query = query.Where(x => x.DataRecebimento == DateTime.Now.AddDays(Convert.ToDouble(model.CustomFilter.Prazo)));

            if (model.CustomFilter.IdStatus != null && model.CustomFilter.IdStatus != 0)
                query = query.Where(x => x.LoteStatus.IdLoteStatus == model.CustomFilter.IdStatus);

            if (query.Count() > 0)
            {
                foreach (var item in query)
                {
                    int? atraso = null;
                    
                    if (item.DataCompra != null)
                    {
                        TimeSpan? data = DateTime.Now - item.DataCompra;
                        atraso = data.Value.Days;
                    }
                    
                    boRecebimentoNotaListItemViewModel.Add(new BORecebimentoNotaListItemViewModel()
                    {
                        Lote = item.IdLote,
                        Nota = item.NotaFiscal.Numero,
                        Fornecedor = item.NotaFiscal.Fornecedor.NomeFantasia,
                        QuantidadePeca = item.QuantidadePeca,
                        QuantidadeVolume = item.QuantidadeVolume,
                        Status = item.LoteStatus.Descricao,
                        Prazo = item.DataCompra.ToString(),
                        Atraso = atraso
                    });
                }
            }

            return DataTableResult.FromModel(new DataTableResponseModel()
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = totalRecordsFiltered,
                Data = boRecebimentoNotaListItemViewModel
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
using FWLog.Data;
using FWLog.Data.Models.FilterCtx;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.BOQuarentenaCtx;
using FWLog.Web.Backoffice.Models.CommonCtx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class BOQuarentenaController : BOBaseController
    {
        private readonly UnitOfWork _uow;

        public BOQuarentenaController(UnitOfWork uow)
        {
            _uow = uow;
        }

        // GET: BOQuarentena
        public ActionResult Index()
        {
            var model = new BOQuarentenaListViewModel
            {
                Filter = new BOQuarentenaFilterViewModel()
                {
                    ListaQuarentenaStatus = new SelectList(
                    _uow.QuarentenaStatusRepository.Todos().Select(x => new SelectListItem
                    {
                        Value = x.IdQuarentenaStatus.ToString(),
                        Text = x.Descricao,
                    }), "Value", "Text"
                )
                }
            };

            return View(model);
        }

        public ActionResult PageData(DataTableFilter<BOQuarentenaFilterViewModel> model)
        {
            List<BOQuarentenaListItemViewModel> boQuarentenaListItemViewModel = new List<BOQuarentenaListItemViewModel>();
            int totalRecords = 0;
            int totalRecordsFiltered = 0;

            var query = _uow.QuarentenaRepository.All();

            totalRecords = query.Count();

            if (!string.IsNullOrEmpty(model.CustomFilter.DANFE))
                query = query.Where(x => x.Lote.NotaFiscal.DANFE.Contains(model.CustomFilter.DANFE));

            if (model.CustomFilter.Lote.HasValue)
                query = query.Where(x => x.IdLote == Convert.ToInt32(model.CustomFilter.Lote));

            if (model.CustomFilter.Nota.HasValue)
                query = query.Where(x => x.Lote.NotaFiscal.Numero == model.CustomFilter.Nota);

            if (model.CustomFilter.IdFornecedor.HasValue)
                query = query.Where(x => x.Lote.NotaFiscal.Fornecedor.IdFornecedor == model.CustomFilter.IdFornecedor);

            if (model.CustomFilter.IdQuarentenaStatus.HasValue)
                query = query.Where(x => x.QuarentenaStatus.IdQuarentenaStatus == model.CustomFilter.IdQuarentenaStatus);

            if (model.CustomFilter.DataAberturaInicial.HasValue)
            {
                DateTime dataInicial = new DateTime(model.CustomFilter.DataAberturaInicial.Value.Year, model.CustomFilter.DataAberturaInicial.Value.Month, model.CustomFilter.DataAberturaInicial.Value.Day,
                    00, 00, 00);
                query = query.Where(x => x.DataAbertura >= dataInicial);
            }

            if (model.CustomFilter.DataAberturaFinal.HasValue)
            {
                DateTime dataFinal = new DateTime(model.CustomFilter.DataAberturaFinal.Value.Year, model.CustomFilter.DataAberturaFinal.Value.Month, model.CustomFilter.DataAberturaFinal.Value.Day,
                    23, 59, 59);
                query = query.Where(x => x.DataAbertura <= dataFinal);
            }

            if (model.CustomFilter.DataEncerramentoInicial.HasValue)
            {
                DateTime dataEncerramentoInicial = new DateTime(model.CustomFilter.DataEncerramentoInicial.Value.Year, model.CustomFilter.DataEncerramentoInicial.Value.Month, model.CustomFilter.DataEncerramentoInicial.Value.Day,
                    00, 00, 00);
                query = query.Where(x => x.DataEncerramento >= dataEncerramentoInicial);
            }

            if (model.CustomFilter.DataEncerramentoFinal.HasValue)
            {
                DateTime prazoEncerramentoFinal = new DateTime(model.CustomFilter.DataEncerramentoFinal.Value.Year, model.CustomFilter.DataEncerramentoFinal.Value.Month, model.CustomFilter.DataEncerramentoFinal.Value.Day,
                    23, 59, 59);
                query = query.Where(x => x.DataEncerramento <= prazoEncerramentoFinal);
            }

            if (query.Any())
            {
                foreach (var item in query)
                {
                    //Atribui 0 para dias em atraso.
                    long? atraso = 0;

                    //Se a data de encerramento NÃO for nula, captura a quantidae de dias entre a data do encerramento e a data de abertura.
                    if (item.DataEncerramento.HasValue)
                    {
                        atraso = (item.DataEncerramento - item.DataAbertura).Value.Days;
                    }
                    else //Se a data de encerramento for nula, ccaptura a quantidade de dias entre a data atual e a data de dencerramento
                    {
                        if (DateTime.Now > item.DataAbertura)
                            atraso = (DateTime.Now - item.DataAbertura).Days;
                    }

                    boQuarentenaListItemViewModel.Add(new BOQuarentenaListItemViewModel()
                    {
                        Lote = item.IdLote,
                        Nota = item.Lote.NotaFiscal.Numero,
                        Fornecedor = item.Lote.NotaFiscal.Fornecedor.NomeFantasia,
                        Atraso = atraso,
                        DataAbertura = item.DataAbertura.ToString("dd/MM/yyyy"),
                        DataEncerramento = item.DataEncerramento.HasValue ? item.DataEncerramento.Value.ToString("dd/MM/yyyy") : string.Empty
                    });
                }
            }

            if (model.CustomFilter.Atraso.HasValue)
            {
                boQuarentenaListItemViewModel = boQuarentenaListItemViewModel.Where(x => x.Atraso == model.CustomFilter.Atraso).ToList();
            }

            totalRecordsFiltered = boQuarentenaListItemViewModel.Count;

            var result = boQuarentenaListItemViewModel
                .OrderBy(model.OrderByColumn, model.OrderByDirection)
                .Skip(model.Start)
                .Take(model.Length);

            return DataTableResult.FromModel(new DataTableResponseModel()
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = totalRecordsFiltered,
                Data = result
            });
        }
    }
}
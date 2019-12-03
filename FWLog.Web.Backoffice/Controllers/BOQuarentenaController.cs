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
                        Text = x.Descricao
                    }), "Value", "Text")
                }
            };

            return View(model);
        }

        public ActionResult PageData(DataTableFilter<BOQuarentenaFilterViewModel> model)
        {
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

            IEnumerable<BOQuarentenaListItemViewModel> list = query.ToList()
                .Select(x => new BOQuarentenaListItemViewModel
                {
                    IdQuarentena = x.IdQuarentena,
                    Lote = x.IdLote,
                    Nota = x.Lote.NotaFiscal.Numero,
                    Fornecedor = x.Lote.NotaFiscal.Fornecedor.NomeFantasia,
                    DataAbertura = x.DataAbertura.ToString("dd/MM/yyyy"),
                    DataEncerramento = x.DataEncerramento.HasValue ? x.DataEncerramento.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Atraso = x.DataAbertura.Subtract(x.DataEncerramento ?? DateTime.Now).Days
                });

            if (model.CustomFilter.Atraso.HasValue)
            {
                list = list.Where(x => x.Atraso == model.CustomFilter.Atraso);
            }

            totalRecordsFiltered = list.Count();

            var result = list
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

        public JsonResult ValidarModalDetalhesQuarentena(long id)
        {
            bool existe = _uow.QuarentenaRepository.Any(x => x.IdQuarentena == id);

            if (!existe)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Não foi possível buscar Nota Fiscal."
                });
            }

            return Json(new AjaxGenericResultModel
            {
                Success = true
            });
        }

        public ActionResult ExibirModalDetalhesQuarentena(long id)
        {
            var entidade = _uow.QuarentenaRepository.All().FirstOrDefault(x => x.IdQuarentena == id);

            var model = new DetalhesQuarentenaViewModel
            {
                IdQuarentena = entidade.IdQuarentena,
                IdStatus = entidade.QuarentenaStatus.IdQuarentenaStatus,
                DataAbertura = entidade.DataAbertura.ToString("dd/MM/yyyy"),
                DataEncerramento = entidade.DataEncerramento.HasValue ? entidade.DataEncerramento.Value.ToString("dd/MM/yyyy") : string.Empty,
                Observacao = entidade.Observacao
            };

            return PartialView("DetalhesQuarentena", model);
            //DetalhesQuarentenaViewModel
        }
    }
}
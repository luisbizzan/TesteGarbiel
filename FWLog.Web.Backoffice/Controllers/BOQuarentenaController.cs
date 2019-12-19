using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Models.GeneralCtx;
using FWLog.Services.Services;
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
        private readonly BOLogSystemService _boLogSystemService;
        private readonly QuarentenaService _quarentenaService;

        public BOQuarentenaController(UnitOfWork uow, BOLogSystemService boLogSystemService, QuarentenaService quarentenaService)
        {
            _uow = uow;
            _boLogSystemService = boLogSystemService;
            _quarentenaService = quarentenaService;
        }

        private SelectList Status
        {
            get
            {
                if (status == null)
                {
                    status = new SelectList(_uow.QuarentenaStatusRepository.Todos().Select(x => new SelectListItem
                    {
                        Value = x.IdQuarentenaStatus.ToString(),
                        Text = x.Descricao
                    }), "Value", "Text");
                }

                return status;
            }
        }
        private SelectList status;

        private void setViewBags()
        {
            ViewBag.Status = Status;
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

            if (!string.IsNullOrEmpty(model.CustomFilter.ChaveAcesso))
                query = query.Where(x => x.Lote.NotaFiscal.ChaveAcesso.Contains(model.CustomFilter.ChaveAcesso));

            if (model.CustomFilter.Lote.HasValue)
                query = query.Where(x => x.IdLote == Convert.ToInt32(model.CustomFilter.Lote));

            if (model.CustomFilter.Nota.HasValue)
                query = query.Where(x => x.Lote.NotaFiscal.Numero == model.CustomFilter.Nota);

            if (model.CustomFilter.IdFornecedor.HasValue)
                query = query.Where(x => x.Lote.NotaFiscal.Fornecedor.IdFornecedor == model.CustomFilter.IdFornecedor);

            if (model.CustomFilter.IdQuarentenaStatus.HasValue)
                query = query.Where(x => (int)x.QuarentenaStatus.IdQuarentenaStatus == model.CustomFilter.IdQuarentenaStatus);

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
                    Atraso = x.DataAbertura.Subtract(x.DataEncerramento ?? DateTime.Now).Days,
                    Status = x.QuarentenaStatus.Descricao
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

        [HttpGet]
        public ActionResult ExibirModalDetalhesQuarentena(long id)
        {
            setViewBags();

            var entidade = _uow.QuarentenaRepository.GetById(id);

            var model = new DetalhesQuarentenaViewModel
            {
                IdQuarentena = entidade.IdQuarentena,
                IdStatus = entidade.QuarentenaStatus.IdQuarentenaStatus,
                DataAbertura = entidade.DataAbertura.ToString("dd/MM/yyyy"),
                DataEncerramento = entidade.DataEncerramento.HasValue ? entidade.DataEncerramento.Value.ToString("dd/MM/yyyy") : string.Empty,
                Observacao = entidade.Observacao
            };


            return PartialView("DetalhesQuarentena", model);
        }

        [HttpPost]
        public ActionResult ExibirModalDetalhesQuarentena(DetalhesQuarentenaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                setViewBags();

                return PartialView("DetalhesQuarentena", model);
            }

            try
            {
                Quarentena entidade = _uow.QuarentenaRepository.GetById(model.IdQuarentena);

                Quarentena old = new Quarentena
                {
                    DataAbertura = entidade.DataAbertura,
                    DataEncerramento = entidade.DataEncerramento,
                    IdLote = entidade.IdLote,
                    IdQuarentena = entidade.IdQuarentena,
                    IdQuarentenaStatus = entidade.IdQuarentenaStatus,
                    Observacao = entidade.Observacao
                };

                entidade.IdQuarentenaStatus = model.IdStatus;
                entidade.Observacao = model.Observacao;

                if (!model.PermiteEdicao)
                {
                    entidade.DataEncerramento = DateTime.Now;
                }

                _uow.QuarentenaRepository.Update(entidade);

                var userInfo = new BackOfficeUserInfo();
                _boLogSystemService.Add(new BOLogSystemCreation
                {
                    ActionType = ActionTypeNames.Edit,
                    IP = userInfo.IP,
                    UserId = userInfo.UserId,
                    EntityName = nameof(Quarentena),
                    OldEntity = old,
                    NewEntity = entidade
                });

                Notify.Success(Resources.CommonStrings.RegisterEditedSuccessMessage);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                setViewBags();

                Notify.Error(Resources.CommonStrings.RegisterEditedErrorMessage);
                return PartialView("DetalhesQuarentena", model);
            }
        }

        public ActionResult TermoResponsabilidade(long idQuarentena)
        {
            var request = new TermoResponsabilidadeRequest
            {
                IdQuarentena = idQuarentena,
                NomeUsuario = User.Identity.Name
            };

            byte[] relatorio = _quarentenaService.TermoResponsabilidade(request);

            return File(relatorio, "application/pdf", "TermoResponsabilidade.pdf");
        }
    }
}
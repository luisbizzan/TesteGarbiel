using FWLog.Services.Model.Relatorios;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx;
using AutoMapper;
using FWLog.Data;
using FWLog.Data.Models.FilterCtx;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class BORecebimentoNotaController : BOBaseController
    {

        private readonly RelatorioService _relatorioService;
        private readonly UnitOfWork _uow;

        public BORecebimentoNotaController(UnitOfWork uow, RelatorioService relatorioService)
        {
            _relatorioService = relatorioService;
            _uow = uow;
        }

        [HttpGet]
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

        [HttpGet]
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

        [HttpPost]
        public ActionResult DownloadRelatorioNotas(BODownloadRelatorioNotasViewModel viewModel)
        {
            ValidateModel(viewModel);

            var relatorioRequest = new RelatorioRecebimentoNotasRequest
            {
                Nota = viewModel.Nota
            };

            byte[] relatorio = _relatorioService.GerarRelatorioRecebimentoNotas(relatorioRequest);

            return File(relatorio, "application/pdf", "Relatório Recebimento Notas.pdf");
        }

        public ActionResult ExibirModalRegistroRecebimento()
        {
            var modal = new BORegistroRecebimentoViewModel();

            return PartialView("RegistroRecebimento", modal);

        }

        public ActionResult CarregarDadosNotaFiscalRegistro(string chaveAcesso, long idLote)
        {
            var model = new BORegistroRecebimentoViewModel();

            var lote = _uow.LoteRepository.GetById(idLote);

            if (lote.NotaFiscal.Chave != chaveAcesso)
            {
                ModelState.AddModelError(nameof(model.ChaveAcesso), "A Chave de Acesso não condiz com a chave cadastrada na nota fiscal do Lote selecionado.");
                return PartialView("RegistroRecebimentoDetalhes", model);
            }

            var dataAtual = DateTime.UtcNow;

            model.ChaveAcesso = lote.NotaFiscal.Chave;
            model.DataRecebimento = dataAtual.ToString("dd/MM/yyyy");
            model.HoraRecebimento = dataAtual.ToString("HH:mm:ss");
            model.FornecedorNome = lote.NotaFiscal.Fornecedor.RazaoSocial;
            model.NumeroSerieNotaFiscal = string.Format("{0}-{1}", lote.NotaFiscal.Numero, lote.NotaFiscal.Serie);
            model.ValorTotal = lote.NotaFiscal.ValorTotal;
            model.ValorFrete = lote.NotaFiscal.ValorFrete;
            model.NumeroConhecimento = lote.NotaFiscal.NumeroConhecimento;
            model.QtdVolumes = lote.NotaFiscal.Quantidade;
            model.TransportadoraNome = lote.NotaFiscal.Transportadora.RazaoSocial;
            model.Peso = lote.NotaFiscal.PesoBruto;

            return PartialView("RegistroRecebimentoDetalhes", model);
        }

        public ActionResult RegistrarRecebimentoNota(long idLote)
        {
            var modal = new BORegistroRecebimentoViewModel();
            return PartialView("RegistroRecebimento", modal);
        }
    }
}
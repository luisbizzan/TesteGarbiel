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
using FWLog.Data.Models;
using System.Net.Sockets;
using System.Net;
using System.Globalization;

namespace FWLog.Web.Backoffice.Controllers
{
    public class BORecebimentoNotaController : BOBaseController
    {

        private readonly RelatorioService _relatorioService;
        private readonly LoteService _LoteService;
        private readonly UnitOfWork _uow;

        public BORecebimentoNotaController(UnitOfWork uow, RelatorioService relatorioService, LoteService loteService)
        {
            _LoteService = loteService;
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

            if (model.CustomFilter.IdFornecedor != null && model.CustomFilter.IdFornecedor != 0)
                query = query.Where(x => x.NotaFiscal.Fornecedor.IdFornecedor == model.CustomFilter.IdFornecedor);

            if (model.CustomFilter.QuantidadePeca != null && model.CustomFilter.QuantidadePeca != 0)
                query = query.Where(x => x.QuantidadePeca == model.CustomFilter.QuantidadePeca);

            if (model.CustomFilter.QuantidadeVolume != null && model.CustomFilter.QuantidadeVolume != 0)
                query = query.Where(x => x.QuantidadeVolume == model.CustomFilter.QuantidadeVolume);

            if (model.CustomFilter.IdStatus != null && model.CustomFilter.IdStatus != 0)
                query = query.Where(x => x.LoteStatus.IdLoteStatus == model.CustomFilter.IdStatus);

            if (model.CustomFilter.Prazo != null && model.CustomFilter.Prazo != 0)
                query = query.Where(x => x.DataRecebimento == DateTime.Now.AddDays(Convert.ToDouble(model.CustomFilter.Prazo)));

            if (query.Count() > 0)
            {
                foreach (var item in query)
                {
                    long? atraso = null;

                    if (item.NotaFiscal.DataEmissao != null)
                    {
                        TimeSpan? data = DateTime.Now - item.NotaFiscal.DataEmissao;
                        atraso = data.Value.Days;
                    }

                    boRecebimentoNotaListItemViewModel.Add(new BORecebimentoNotaListItemViewModel()
                    {
                        Lote = item.IdLote == 0 ? (long?)null : item.IdLote,
                        Nota = item.NotaFiscal.Numero == 0 ? (long?)null : item.NotaFiscal.Numero,
                        Fornecedor = item.NotaFiscal.Fornecedor.NomeFantasia,
                        QuantidadePeca = item.QuantidadePeca == 0 ? (long?)null : item.QuantidadePeca,
                        QuantidadeVolume = item.QuantidadeVolume == 0 ? (long?)null : item.QuantidadeVolume,
                        Status = item.LoteStatus.Descricao,
                        Prazo = item.NotaFiscal.DataEmissao.ToString(),
                        Atraso = atraso,
                        IdNotaFiscal = item.NotaFiscal.IdNotaFiscal
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
                Lote = viewModel.Lote,
                Nota = viewModel.Nota,
                DANFE = viewModel.DANFE,
                IdStatus = viewModel.IdStatus,
                DataInicial = viewModel.DataInicial,
                DataFinal = viewModel.DataFinal,
                PrazoInicial = viewModel.PrazoInicial,
                PrazoFinal = viewModel.PrazoFinal,
                IdFornecedor = viewModel.IdFornecedor,
                Atraso = viewModel.Atraso,
                QuantidadePeca = viewModel.QuantidadePeca,
                Volume = viewModel.Volume
            };

            byte[] relatorio = _relatorioService.GerarRelatorioRecebimentoNotas(relatorioRequest);

            return File(relatorio, "application/pdf", "Relatório Recebimento Notas.pdf");
        }

        public ActionResult ExibirModalRegistroRecebimento(long id)
        {
            var modal = new BORegistroRecebimentoViewModel();

            modal.IdNotaFiscal = id;

            return PartialView("RegistroRecebimento", modal);

        }

        public ActionResult CarregarDadosNotaFiscalRegistro(string chaveAcesso, string idNotaFiscal)
        {
            var model = new BORegistroRecebimentoViewModel();
             
            var notafiscal = _uow.NotaFiscalRepository.GetById(Convert.ToInt64(idNotaFiscal));

            if (notafiscal.Chave != chaveAcesso)
            {
                ModelState.AddModelError(nameof(model.ChaveAcesso), "A Chave de Acesso não condiz com a chave cadastrada na nota fiscal do Lote selecionado.");
                //TODO Ajustar
                return PartialView("RegistroRecebimentoDetalhes", model);
            }

            var dataAtual = DateTime.UtcNow;
                       
            model.ChaveAcesso = notafiscal.Chave;
            model.DataRecebimento = dataAtual.ToString("dd/MM/yyyy");
            model.HoraRecebimento = dataAtual.ToString("HH:mm:ss");
            model.FornecedorNome = notafiscal.Fornecedor.RazaoSocial;
            model.NumeroSerieNotaFiscal = string.Format("{0}-{1}", notafiscal.Numero, notafiscal.Serie);
            model.ValorTotal = notafiscal.ValorTotal;

            if (notafiscal.FreteTipo.Sigla == "F")
            {
                model.ValorFrete = notafiscal.ValorFrete;
                model.NumeroConhecimento = notafiscal.NumeroConhecimento;               
                model.TransportadoraNome = notafiscal.Transportadora.RazaoSocial;
                model.Peso = notafiscal.PesoBruto;
                model.ExibeCamposFrete = false;
            }
            else
            {
                model.ExibeCamposFrete = true;
            }
         
            return PartialView("RegistroRecebimentoDetalhes", model);
        }

        public ActionResult RegistrarRecebimentoNota(long idNotaFiscal, Decimal ValorFrete, string nroConhecimento, int idTransportadora, Decimal peso, int volumes, DateTime dataRecebimento)
        {
            var userInfo = new BackOfficeUserInfo();
            _LoteService.RegistrarRecebimentoNotaFiscal(idNotaFiscal, userInfo.UserId.ToString(), dataRecebimento);

            var modal = new BORegistroRecebimentoViewModel();
            return PartialView("RegistroRecebimento", modal);
        }

        [HttpPost]
        public JsonResult ImprimirRelatorioNotas(BOImprimirRelatorioNotasViewModel viewModel)
        {
            try
            {
                ValidateModel(viewModel);

                var relatorioRequest = new RelatorioRecebimentoNotasRequest
                {
                    Lote = viewModel.Lote,
                    Nota = viewModel.Nota,
                    DANFE = viewModel.DANFE,
                    IdStatus = viewModel.IdStatus,
                    DataInicial = viewModel.DataInicial,
                    DataFinal = viewModel.DataFinal,
                    PrazoInicial = viewModel.PrazoInicial,
                    PrazoFinal = viewModel.PrazoFinal,
                    IdFornecedor = viewModel.IdFornecedor,
                    Atraso = viewModel.Atraso,
                    QuantidadePeca = viewModel.QuantidadePeca,
                    Volume = viewModel.Volume
                };

                byte[] relatorio = _relatorioService.GerarRelatorioRecebimentoNotas(relatorioRequest);

                Printer impressora = _uow.BOPrinterRepository.GetById(viewModel.IdImpressora);
                var ipPorta = impressora.IP.Split(':');

                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    NoDelay = true
                };

                IPAddress ip = IPAddress.Parse(ipPorta[0]);
                IPEndPoint ipep = new IPEndPoint(ip, int.Parse(ipPorta[1]));
                clientSocket.Connect(ipep);

                NetworkStream ns = new NetworkStream(clientSocket);
                ns.Write(relatorio, 0, relatorio.Length);
                ns.Close();
                clientSocket.Close();

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Impressão enviada com sucesso."
                }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Ocorreu um erro na impressão."
                }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public ActionResult DetalhesEntradaConferencia(long id)
        {
            NotaFiscal notaFiscal = _uow.NotaFiscalRepository.GetById(id);

            var model = new BODetalhesEntradaConferenciaViewModel();

            return View(model);
        }
    }
}
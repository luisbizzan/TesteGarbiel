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
            List<BORecebimentoNotaListItemViewModel> boRecebimentoNotaListItemViewModel = new List<BORecebimentoNotaListItemViewModel>();
            int totalRecords = 0;
            int totalRecordsFiltered = 0;

            var query = _uow.LoteRepository.Obter(CompanyId).AsQueryable();

            totalRecords = query.Count();

            if (!string.IsNullOrEmpty(model.CustomFilter.DANFE))
                query = query.Where(x => x.NotaFiscal.DANFE == model.CustomFilter.DANFE);

            if (model.CustomFilter.Lote.HasValue)
                query = query.Where(x => x.IdLote == Convert.ToInt32(model.CustomFilter.Lote));

            if (model.CustomFilter.Nota.HasValue)
                query = query.Where(x => x.NotaFiscal.Numero == model.CustomFilter.Nota);

            if (model.CustomFilter.IdFornecedor.HasValue)
                query = query.Where(x => x.NotaFiscal.Fornecedor.IdFornecedor == model.CustomFilter.IdFornecedor);

            if (model.CustomFilter.QuantidadePeca.HasValue)
                query = query.Where(x => x.NotaFiscal.Quantidade == model.CustomFilter.QuantidadePeca);

            if (model.CustomFilter.QuantidadeVolume.HasValue)
                query = query.Where(x => x.QuantidadeVolume == model.CustomFilter.QuantidadeVolume);

            if (model.CustomFilter.IdStatus.HasValue)
                query = query.Where(x => x.LoteStatus.IdLoteStatus == model.CustomFilter.IdStatus);

            if (model.CustomFilter.DataInicial.HasValue)
            {
                DateTime dataInicial = new DateTime(model.CustomFilter.DataInicial.Value.Year, model.CustomFilter.DataInicial.Value.Month, model.CustomFilter.DataInicial.Value.Day,
                    00, 00, 00);
                query = query.Where(x => x.DataRecebimento >= dataInicial);
            }

            if (model.CustomFilter.DataFinal.HasValue)
            {
                DateTime dataFinal = new DateTime(model.CustomFilter.DataFinal.Value.Year, model.CustomFilter.DataFinal.Value.Month, model.CustomFilter.DataFinal.Value.Day,
                    23, 59, 59);
                query = query.Where(x => x.DataRecebimento <= dataFinal);
            }

            if (model.CustomFilter.PrazoInicial.HasValue)
            {
                DateTime prazoInicial = new DateTime(model.CustomFilter.PrazoInicial.Value.Year, model.CustomFilter.PrazoInicial.Value.Month, model.CustomFilter.PrazoInicial.Value.Day,
                    00, 00, 00);
                query = query.Where(x => x.NotaFiscal.PrazoEntregaFornecedor >= prazoInicial);
            }

            if (model.CustomFilter.PrazoFinal.HasValue)
            {
                DateTime prazoFinal = new DateTime(model.CustomFilter.PrazoFinal.Value.Year, model.CustomFilter.PrazoFinal.Value.Month, model.CustomFilter.PrazoFinal.Value.Day,
                    23, 59, 59);
                query = query.Where(x => x.NotaFiscal.PrazoEntregaFornecedor <= prazoFinal);
            }

            if (query.Count() > 0)
            {
                foreach (var item in query)
                {
                    //Atribui 0 para dias em atraso.
                    long? atraso = 0;

                    //Se a mercadoria for igual a null, o atraso já será considerado 0. Caso contrário, entra no IF para outras validações.
                    if (item.NotaFiscal.PrazoEntregaFornecedor != null)
                    {
                        //Atribui o prazo de entrega da nota fiscal.
                        DateTime prazoEntrega = item.NotaFiscal.PrazoEntregaFornecedor;

                        //Se a data de recebimento for nula, compara a data atual com o prazo de entrega para calcular os dias em atraso.
                        if (!item.DataRecebimento.HasValue)
                            if (DateTime.Now > prazoEntrega)
                                atraso = (DateTime.Now - prazoEntrega).Days;
                            else //Se a data de recebimento NÃO for nula, compara a data do recebimento com o prazo de entrega para calcular os dias em atraso.
                            {
                                if (item.DataRecebimento > prazoEntrega)
                                atraso = (item.DataRecebimento - prazoEntrega).Value.Days;
                            }
                    }

                    boRecebimentoNotaListItemViewModel.Add(new BORecebimentoNotaListItemViewModel()
                    {
                        Lote = item.IdLote == 0 ? (long?)null : item.IdLote,
                        Nota = item.NotaFiscal.Numero == 0 ? (long?)null : item.NotaFiscal.Numero,
                        Fornecedor = item.NotaFiscal.Fornecedor.NomeFantasia,
                        QuantidadePeca = item.NotaFiscal.Quantidade == 0 ? (long?)null : item.NotaFiscal.Quantidade,
                        QuantidadeVolume = item.QuantidadeVolume == 0 ? (long?)null : item.QuantidadeVolume,
                        RecebidoEm = item.DataRecebimento.ToString(),
                        Status = item.LoteStatus.Descricao,
                        Prazo = item.NotaFiscal.PrazoEntregaFornecedor.ToString("dd/MM/yyyy"),
                        Atraso = atraso,
                        IdUsuarioRecebimento = item.UsuarioRecebimento == null ? "" : item.UsuarioRecebimento.Id
                    });
                }
            }

            if (model.CustomFilter.Atraso.HasValue)
            {
                boRecebimentoNotaListItemViewModel = boRecebimentoNotaListItemViewModel.Where(x => x.Atraso == model.CustomFilter.Atraso).ToList();                
            }

            if (!String.IsNullOrEmpty(model.CustomFilter.IdUsuarioRecebimento))
                boRecebimentoNotaListItemViewModel = boRecebimentoNotaListItemViewModel.Where(x => x.IdUsuarioRecebimento == model.CustomFilter.IdUsuarioRecebimento).ToList();

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
    }
}
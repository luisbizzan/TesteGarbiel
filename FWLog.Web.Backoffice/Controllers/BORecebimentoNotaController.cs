using AutoMapper;
using ExtensionMethods.List;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Model.Relatorios;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx;
using FWLog.Web.Backoffice.Models.CommonCtx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class BORecebimentoNotaController : BOBaseController
    {
        private readonly RelatorioService _relatorioService;
        private readonly LoteService _loteService;
        private readonly ApplicationLogService _applicationLogService;
        private readonly UnitOfWork _uow;

        public BORecebimentoNotaController(UnitOfWork uow, RelatorioService relatorioService, LoteService loteService, ApplicationLogService applicationLogService)
        {
            _loteService = loteService;
            _relatorioService = relatorioService;
            _applicationLogService = applicationLogService;
            _uow = uow;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var model = new BORecebimentoNotaListViewModel
            {
                Filter = new BORecebimentoNotaFilterViewModel()
                {
                    ListaStatus = new SelectList(
                    _uow.LoteStatusRepository.Todos().Select(x => new SelectListItem
                    {
                        Value = x.IdLoteStatus.ToString(),
                        Text = x.Descricao,
                    }), "Value", "Text"
                )
                }
            };

            model.Filter.IdStatus = StatusNotaRecebimento.AguardandoRecebimento.GetHashCode();
            model.Filter.PrazoInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
            model.Filter.PrazoFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00).AddDays(10);

            return View(model);
        }

        [HttpPost]
        public ActionResult PageData(DataTableFilter<BORecebimentoNotaFilterViewModel> model)
        {
            List<BORecebimentoNotaListItemViewModel> boRecebimentoNotaListItemViewModel = new List<BORecebimentoNotaListItemViewModel>();
            int totalRecords = 0;
            int totalRecordsFiltered = 0;

            if (!ModelState.IsValid)
            {
                return DataTableResult.FromModel(new DataTableResponseModel()
                {
                    Draw = model.Draw,
                    RecordsTotal = totalRecords,
                    RecordsFiltered = totalRecordsFiltered,
                    Data = boRecebimentoNotaListItemViewModel
                });
            }

            var query = _uow.LoteRepository.Obter(IdEmpresa).AsQueryable();

            totalRecords = query.Count();

            if (!string.IsNullOrEmpty(model.CustomFilter.ChaveAcesso))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.NotaFiscal.ChaveAcesso) && x.NotaFiscal.ChaveAcesso.Contains(model.CustomFilter.ChaveAcesso));
            }

            if (model.CustomFilter.Lote.HasValue)
            {
                query = query.Where(x => x.IdLote == Convert.ToInt32(model.CustomFilter.Lote));
            }

            if (model.CustomFilter.Nota.HasValue)
            {
                query = query.Where(x => x.NotaFiscal.Numero == model.CustomFilter.Nota);
            }

            if (model.CustomFilter.IdFornecedor.HasValue)
            {
                query = query.Where(x => x.NotaFiscal.Fornecedor.IdFornecedor == model.CustomFilter.IdFornecedor);
            }

            if (model.CustomFilter.QuantidadePeca.HasValue)
            {
                query = query.Where(x => x.NotaFiscal.Quantidade == model.CustomFilter.QuantidadePeca);
            }

            if (model.CustomFilter.QuantidadeVolume.HasValue)
            {
                query = query.Where(x => x.QuantidadeVolume == model.CustomFilter.QuantidadeVolume);
            }

            if (model.CustomFilter.IdStatus.HasValue)
            {
                query = query.Where(x => x.LoteStatus.IdLoteStatus == model.CustomFilter.IdStatus);
            }

            if (model.CustomFilter.DataInicial.HasValue)
            {
                DateTime dataInicial = new DateTime(model.CustomFilter.DataInicial.Value.Year, model.CustomFilter.DataInicial.Value.Month, model.CustomFilter.DataInicial.Value.Day, 00, 00, 00);
                query = query.Where(x => x.DataRecebimento >= dataInicial);
            }

            if (model.CustomFilter.DataFinal.HasValue)
            {
                DateTime dataFinal = new DateTime(model.CustomFilter.DataFinal.Value.Year, model.CustomFilter.DataFinal.Value.Month, model.CustomFilter.DataFinal.Value.Day, 23, 59, 59);
                query = query.Where(x => x.DataRecebimento <= dataFinal);
            }

            if (model.CustomFilter.PrazoInicial != null)
            {
                DateTime prazoInicial = new DateTime(model.CustomFilter.PrazoInicial.Year, model.CustomFilter.PrazoInicial.Month, model.CustomFilter.PrazoInicial.Day,
                    00, 00, 00);
                query = query.Where(x => x.NotaFiscal.PrazoEntregaFornecedor >= prazoInicial);
            }

            if (model.CustomFilter.PrazoFinal != null)
            {
                DateTime prazoFinal = new DateTime(model.CustomFilter.PrazoFinal.Year, model.CustomFilter.PrazoFinal.Month, model.CustomFilter.PrazoFinal.Day,
                    23, 59, 59);
                query = query.Where(x => x.NotaFiscal.PrazoEntregaFornecedor <= prazoFinal);
            }

            if (query.Any())
            {
                foreach (var item in query)
                {
                    //Atribui 0 para dias em atraso.
                    long? atraso = 0;

                    //Se o prazo de entrega do fornecedor for igual a null, o atraso já será considerado 0. Caso contrário, entra no IF para outras validações.
                    if (item.NotaFiscal.PrazoEntregaFornecedor != null)
                    {
                        //Atribui o prazo de entrega da nota fiscal.
                        DateTime prazoEntrega = item.NotaFiscal.PrazoEntregaFornecedor;

                        //Se a data de recebimento for nula, captura a quantidade de dias entre o prazo de entrega e a data atual.
                        if (item.LoteStatus.IdLoteStatus == StatusNotaRecebimento.AguardandoRecebimento.GetHashCode())
                        {
                            if (DateTime.Now > prazoEntrega)
                            {
                                atraso = DateTime.Now.Subtract(prazoEntrega).Days;
                            }

                        }
                        else //Se a data de recebimento NÃO for nula, captura a quantidade de dias entre o prazo de entrega e a data de recebimento.
                        {
                            if (item.DataRecebimento > prazoEntrega)
                            {
                                atraso = item.DataRecebimento.Subtract(prazoEntrega).Days;
                            }
                        }
                    }

                    boRecebimentoNotaListItemViewModel.Add(new BORecebimentoNotaListItemViewModel()
                    {
                        Lote = item.IdLote == 0 ? (long?)null : item.IdLote,
                        Nota = item.NotaFiscal.Numero == 0 ? (long?)null : item.NotaFiscal.Numero,
                        Fornecedor = item.NotaFiscal.Fornecedor.NomeFantasia,
                        QuantidadePeca = item.NotaFiscal.Quantidade == 0 ? (long?)null : item.NotaFiscal.Quantidade,
                        QuantidadeVolume = item.QuantidadeVolume == 0 ? (long?)null : item.QuantidadeVolume,
                        RecebidoEm = item.LoteStatus.IdLoteStatus != StatusNotaRecebimento.AguardandoRecebimento.GetHashCode() ? item.DataRecebimento.ToString() : "-",
                        Status = item.LoteStatus.Descricao,
                        IdNotaFiscal = item.NotaFiscal.IdNotaFiscal,
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

            if (!string.IsNullOrEmpty(model.CustomFilter.IdUsuarioRecebimento))
            {
                boRecebimentoNotaListItemViewModel = boRecebimentoNotaListItemViewModel.Where(x => x.IdUsuarioRecebimento == model.CustomFilter.IdUsuarioRecebimento).ToList();
            }

            totalRecordsFiltered = boRecebimentoNotaListItemViewModel.Count;

            var result = boRecebimentoNotaListItemViewModel
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

        [HttpGet]
        public ActionResult EntradaConferencia()
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

            var relatorioRequest = Mapper.Map<RelatorioRecebimentoNotasRequest>(viewModel);
            relatorioRequest.IdEmpresa = IdEmpresa;
            relatorioRequest.NomeUsuario = User.Identity.Name;
            byte[] relatorio = _relatorioService.GerarRelatorioRecebimentoNotas(relatorioRequest);

            return File(relatorio, "application/pdf", "Relatório Recebimento Notas.pdf");
        }

        [HttpGet]
        public ActionResult DownloadDetalhesNotaEntradaConferencia(int id)
        {
            var relatorioRequest = new DetalhesNotaEntradaConferenciaRequest
            {
                IdEmpresa = IdEmpresa,
                NomeUsuario = User.Identity.Name,
                IdNotaFiscal = id
            };

            byte[] relatorio = _relatorioService.GerarDetalhesNotaEntradaConferencia(relatorioRequest);

            return File(relatorio, "application/pdf", "Detalhes Nota Fiscal Entrada Conferencia.pdf");
        }

        [HttpPost]
        public JsonResult ImprimirDetalhesEntradaConferencia(BOImprimirDetalhesEntradaConferenciaViewModel viewModel)
        {
            try
            {
                ValidateModel(viewModel);

                var relatorioRequest = new DetalhesNotaEntradaConferenciaRequest
                {
                    IdEmpresa = IdEmpresa,
                    NomeUsuario = User.Identity.Name,
                    IdNotaFiscal = viewModel.IdNotaFiscal
                };

                byte[] relatorio = _relatorioService.GerarDetalhesNotaEntradaConferencia(relatorioRequest);

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
            catch
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Ocorreu um erro na impressão."
                }, JsonRequestBehavior.DenyGet);
            }
        }

        public JsonResult ValidarModalRegistroRecebimento(long id)
        {
            var lote = _uow.LoteRepository.PesquisarLotePorNotaFiscal(id);

            if (lote != null)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Recebimento da mecadoria já efetivado no sistema.",
                });
            }

            return Json(new AjaxGenericResultModel
            {
                Success = true
            });
        }

        [HttpGet]
        public ActionResult ExibirModalRegistroRecebimento(long id)
        {
            var modal = new BORegistroRecebimentoViewModel
            {
                IdNotaFiscal = id
            };

            return PartialView("RegistroRecebimento", modal);
        }

        [HttpPost]
        public JsonResult ValidarNotaFiscalRegistro(string chaveAcesso, long idNotaFiscal)
        {
            var notafiscal = _uow.NotaFiscalRepository.GetById(idNotaFiscal);

            if (notafiscal.ChaveAcesso != chaveAcesso)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "A Chave de Acesso não condiz com a chave cadastrada na nota fiscal do Lote selecionado."
                });
            }

            var lote = _uow.LoteRepository.PesquisarLotePorNotaFiscal(idNotaFiscal);

            if (lote != null)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Recebimento da mecadoria já efetivado no sistema."
                });
            }

            return Json(new AjaxGenericResultModel
            {
                Success = true,
            });
        }

        public ActionResult CarregarDadosNotaFiscalRegistro(string id)
        {
            var notafiscal = _uow.NotaFiscalRepository.GetById(Convert.ToInt64(id));
            var dataAtual = DateTime.Now;

            var model = new BORegistroRecebimentoViewModel
            {
                ChaveAcesso = notafiscal.ChaveAcesso,
                DataRecebimento = dataAtual.ToString("dd/MM/yyyy"),
                HoraRecebimento = dataAtual.ToString("HH:mm:ss"),
                FornecedorNome = notafiscal.Fornecedor.RazaoSocial,
                NumeroSerieNotaFiscal = string.Format("{0}-{1}", notafiscal.Numero, notafiscal.Serie),
                ValorTotal = notafiscal.ValorTotal.ToString("n2"),
                DataAtual = dataAtual,
                ValorFrete = notafiscal.ValorFrete.ToString("n2"),
                NumeroConhecimento = notafiscal.NumeroConhecimento,
                TransportadoraNome = notafiscal.Transportadora.RazaoSocial,
                Peso = notafiscal.PesoBruto.HasValue ? notafiscal.PesoBruto.Value.ToString("n2") : null,
                QtdVolumes = notafiscal.Quantidade == 0 ? (int?)null : notafiscal.Quantidade,
                NotaFiscalPesquisada = true
            };

            return PartialView("RegistroRecebimentoDetalhes", model);
        }

        [HttpPost]
        public async Task<JsonResult> RegistrarRecebimentoNota(long idNotaFiscal, DateTime dataRecebimento, int qtdVolumes, bool notaFiscalPesquisada)
        {
            if (!(idNotaFiscal > 0) || !(qtdVolumes > 0) || !notaFiscalPesquisada)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Selecione a nota fiscal e insira a quantidade de volumes para confirmar o recebimento."
                });
            }

            try
            {
                var userInfo = new BackOfficeUserInfo();
                await _loteService.RegistrarRecebimentoNotaFiscal(idNotaFiscal, userInfo.UserId.ToString(), dataRecebimento, qtdVolumes);
            }
            catch (Exception e)
            {
                _applicationLogService.Error(ApplicationEnum.BackOffice, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Não foi possível atualizar o status da Nota Fiscal no Sankhya. Tente novamente."
                });
            }

            return Json(new AjaxGenericResultModel
            {
                Success = true,
                Message = "Recebimento da nota fiscal registrado com sucesso. Lote gerado"
            });
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
                    ChaveAcesso = viewModel.ChaveAcesso,
                    IdStatus = viewModel.IdStatus,
                    DataInicial = viewModel.DataInicial,
                    DataFinal = viewModel.DataFinal,
                    PrazoInicial = viewModel.PrazoInicial,
                    PrazoFinal = viewModel.PrazoFinal,
                    IdFornecedor = viewModel.IdFornecedor,
                    Atraso = viewModel.Atraso,
                    QuantidadePeca = viewModel.QuantidadePeca,
                    QuantidadeVolume = viewModel.Volume
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

            var model = new BODetalhesEntradaConferenciaViewModel
            {
                IdNotaFiscal = notaFiscal.IdNotaFiscal,
                ChaveAcesso = notaFiscal.ChaveAcesso,
                NumeroNotaFiscal = notaFiscal.Numero.ToString(),
                StatusNotaFiscal = notaFiscal.StatusIntegracao,
                Fornecedor = string.Concat(notaFiscal.Fornecedor.CodigoIntegracao.ToString(), " - ", notaFiscal.Fornecedor.RazaoSocial),
                Quantidade = notaFiscal.Quantidade.ToString(),
                DataCompra = notaFiscal.DataEmissao.ToString("dd/MM/yyyy"),
                PrazoRecebimento = notaFiscal.PrazoEntregaFornecedor.ToString("dd/MM/yyyy"),
                FornecedorCNPJ = notaFiscal.Fornecedor.CNPJ.Substring(0, 2) + "." + notaFiscal.Fornecedor.CNPJ.Substring(2, 3) + "." + notaFiscal.Fornecedor.CNPJ.Substring(5, 3) + "/" + notaFiscal.Fornecedor.CNPJ.Substring(8, 4) + "-" + notaFiscal.Fornecedor.CNPJ.Substring(12, 2),
                ValorTotal = notaFiscal.ValorTotal.ToString("C"),
                ValorFrete = notaFiscal.ValorFrete.ToString("C"),
                NumeroConhecimento = notaFiscal.NumeroConhecimento.ToString(),
                PesoConhecimento = notaFiscal.PesoBruto.HasValue ? notaFiscal.PesoBruto.Value.ToString("F") : null,
                TransportadoraNome = notaFiscal.Transportadora.RazaoSocial,
                DiasAtraso = "0"
            };

            if (notaFiscal.IdNotaFiscalStatus == NotaFiscalStatusEnum.AguardandoRecebimento || notaFiscal.IdNotaFiscalStatus == NotaFiscalStatusEnum.ProcessandoIntegracao)
            {
                model.StatusNotaFiscal = "Aguardando Recebimento";
                model.UsuarioRecebimento = "-";
                model.Volumes = "-";
                model.DataChegada = "-";
                model.NumeroLote = "-";

                if (DateTime.Now > notaFiscal.PrazoEntregaFornecedor)
                {
                    TimeSpan atraso = DateTime.Now.Subtract(notaFiscal.PrazoEntregaFornecedor);
                    model.DiasAtraso = atraso.Days.ToString();
                }
            }
            else
            {
                Lote lote = _uow.LoteRepository.ObterLoteNota(notaFiscal.IdNotaFiscal);

                model.IsNotaRecebida = true;
                model.NumeroLote = lote.IdLote.ToString();
                model.DataChegada = lote.DataRecebimento.ToString("dd/MM/yyyy");
                model.UsuarioRecebimento = lote.UsuarioRecebimento.UserName;
                model.Volumes = lote.QuantidadeVolume.ToString();

                switch (notaFiscal.IdNotaFiscalStatus)
                {
                    case NotaFiscalStatusEnum.Recebida:
                        model.StatusNotaFiscal = "Recebida";
                        break;
                    case NotaFiscalStatusEnum.Conferida:
                        model.StatusNotaFiscal = "Conferida";
                        break;
                    case NotaFiscalStatusEnum.ConferidaDivergencia:
                        model.StatusNotaFiscal = "Conferida com Divergência";
                        break;
                    default:
                        model.StatusNotaFiscal = "Status não cadastrado";
                        break;
                }

                if (lote.DataRecebimento > notaFiscal.PrazoEntregaFornecedor)
                {
                    TimeSpan atraso = lote.DataRecebimento.Subtract(notaFiscal.PrazoEntregaFornecedor);
                    model.DiasAtraso = atraso.Days.ToString();
                }

                if (notaFiscal.IdNotaFiscalStatus == NotaFiscalStatusEnum.Conferida ||
                    notaFiscal.IdNotaFiscalStatus == NotaFiscalStatusEnum.ConferidaDivergencia)
                {
                    model.IsNotaConferida = notaFiscal.IdNotaFiscalStatus == NotaFiscalStatusEnum.Conferida;
                    model.IsNotaDivergente = notaFiscal.IdNotaFiscalStatus == NotaFiscalStatusEnum.ConferidaDivergencia;

                    var loteConferencia = _uow.LoteConferenciaRepository.ObterPorId(lote.IdLote);

                    model.UsuarioConferencia = loteConferencia.UsuarioConferente.UserName;
                    model.DataInicioConferencia = loteConferencia.DataHoraInicio.ToString("dd/MM/YYY HH:mm");
                    model.DataFimConferencia = loteConferencia.DataHoraFim.ToString("dd/MM/YYY HH:mm");
                    model.TempoTotalConferencia = loteConferencia.Tempo.ToString("HH:mm");
                }
                else
                {
                    model.UsuarioConferencia = "Não Conferido";
                    model.DataInicioConferencia = "Não Conferido";
                    model.DataFimConferencia = "Não Conferido";
                    model.TempoTotalConferencia = "Não Conferido";
                }
            }

            List<NotaFiscalItem> listaItensNotaFiscal = _uow.NotaFiscalItemRepository.ObterItens(notaFiscal.IdNotaFiscal);

            model.Items = new List<BODetalhesEntradaConferenciaItem>();

            foreach (NotaFiscalItem notaFiscalItem in listaItensNotaFiscal)
            {
                var entradaConferenciaItem = new BODetalhesEntradaConferenciaItem
                {
                    Referencia = notaFiscalItem.Produto.Referencia,
                    Quantidade = notaFiscalItem.Quantidade.ToString()
                };

                if (notaFiscal.IdNotaFiscalStatus == NotaFiscalStatusEnum.Conferida ||
                    notaFiscal.IdNotaFiscalStatus == NotaFiscalStatusEnum.ConferidaDivergencia)
                {
                    Lote lote = _uow.LoteRepository.ObterLoteNota(notaFiscal.IdNotaFiscal);

                    var loteConferencia = _uow.LoteConferenciaRepository.ObterPorId(lote.IdLote);

                    entradaConferenciaItem.UsuarioConferencia = loteConferencia.UsuarioConferente.UserName;
                    entradaConferenciaItem.DataInicioConferencia = loteConferencia.DataHoraInicio.ToString("dd/MM/YYY HH:mm");
                    entradaConferenciaItem.DataFimConferencia = loteConferencia.DataHoraFim.ToString("dd/MM/YYY HH:mm");
                    entradaConferenciaItem.TempoTotalConferencia = loteConferencia.Tempo.ToString("HH:mm");
                }
                else
                {
                    entradaConferenciaItem.UsuarioConferencia = "Não Conferido";
                    entradaConferenciaItem.DataInicioConferencia = "Não Conferido";
                    entradaConferenciaItem.DataFimConferencia = "Não Conferido";
                    entradaConferenciaItem.TempoTotalConferencia = "Não Conferido";
                }

                model.Items.Add(entradaConferenciaItem);
            }

            return View(model);
        }

        public JsonResult ValidarModalRegistroConferencia(long id)
        {
            NotaFiscal notaFiscal = _uow.NotaFiscalRepository.GetById(id);

            if (notaFiscal == null)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Não foi possível buscar a Nota Fiscal. Por favor, tente novamente!"
                });
            }

            var lote = _uow.LoteRepository.PesquisarLotePorNotaFiscal(id);

            if (lote == null)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "O Lote ainda não foi recebido."
                });
            }
            else
            {
                if (lote.IdLoteStatus == (int)LoteStatusEnum.ConferidoDivergencia)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "O Lote já foi conferido."
                    });
                }
                else if (
                    lote.IdLoteStatus == (int)LoteStatusEnum.Finalizado ||
                    lote.IdLoteStatus == (int)LoteStatusEnum.FinalizadoDivergenciaInvertida ||
                    lote.IdLoteStatus == (int)LoteStatusEnum.FinalizadoDivergenciaNegativa ||
                    lote.IdLoteStatus == (int)LoteStatusEnum.FinalizadoDivergenciaPositiva ||
                    lote.IdLoteStatus == (int)LoteStatusEnum.FinalizadoDivergenciaTodas

                    )
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "O Lote já foi conferido e finalizado."
                    });
                }

                return Json(new AjaxGenericResultModel
                {
                    Success = true
                });
            }
        }


        public ActionResult ExibirModalRegistroConferencia(long id)
        {
            var lote = _uow.LoteRepository.PesquisarLotePorNotaFiscal(id);

            //if (lote == null) Tratar o lote nulo

            var usuarioLogado = new BackOfficeUserInfo();

            ApplicationUser applicationUser = UserManager.Users.FirstOrDefault(x => x.Id == (string)usuarioLogado.UserId);

            var empresaConfig = _uow.EmpresaConfigRepository.ConsultarPorIdEmpresa(IdEmpresa);

            //if (empresaConfig == null)


            var model = new BOEntradaConferenciaViewModel
            {
                IdNotaFiscal = lote.NotaFiscal.IdNotaFiscal,
                IdLote = lote.IdLote,
                NumeroNotaFiscal = lote.NotaFiscal.Numero + lote.NotaFiscal.Serie,
                IdUuarioConferente = applicationUser.Id,
                NomeConferente = applicationUser.UserName,
                DataHoraRecebimento = lote.DataRecebimento.ToString("dd/MM/yyyy HH:mm"),
                NomeFornecedor = lote.NotaFiscal.Fornecedor.NomeFantasia,
                QuantidadeVolume = lote.QuantidadeVolume,
                TipoConferencia = empresaConfig.TipoConferencia.Descricao,
                IdTipoConferencia = empresaConfig.TipoConferencia.IdTipoConferencia.GetHashCode()
            };

            return PartialView("EntradaConferencia", model);
        }

        //public ActionResult ValidarReferenciaConferencia(string referencia)
        //{
            
        //}

        [HttpPost]
        public ActionResult CarregarDadosReferenciaConferencia(string codigoBarrasReferencia, long idLote)
        {
            if (String.IsNullOrEmpty(codigoBarrasReferencia))
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Referência inválida. Por favor, tente novamente!"
                });
            }

            var produto = _uow.ProdutoRepository.ConsultarPorCodigoBarras(codigoBarrasReferencia);

            if (produto == null)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Referência não cadastrada. Por favor, tente novamente!"
                });
            }

            var lote = _uow.LoteRepository.PesquisarLotePorNotaFiscal(idLote);

            //if (lote == null) Tratar o lote nulo

            var usuarioLogado = new BackOfficeUserInfo();

            ApplicationUser applicationUser = UserManager.Users.FirstOrDefault(x => x.Id == (string)usuarioLogado.UserId);

            var empresaConfig = _uow.EmpresaConfigRepository.ConsultarPorIdEmpresa(IdEmpresa);

            //if (empresaConfig == null)

            var model = new BOEntradaConferenciaViewModel
            {
                IdNotaFiscal = lote.NotaFiscal.IdNotaFiscal,
                IdLote = lote.IdLote,
                NumeroNotaFiscal = lote.NotaFiscal.Numero + lote.NotaFiscal.Serie,
                IdUuarioConferente = applicationUser.Id,
                NomeConferente = applicationUser.UserName,
                DataHoraRecebimento = lote.DataRecebimento.ToString("dd/MM/yyyy HH:mm"),
                NomeFornecedor = lote.NotaFiscal.Fornecedor.NomeFantasia,
                QuantidadeVolume = lote.QuantidadeVolume,
                TipoConferencia = empresaConfig.TipoConferencia.Descricao,
                IdTipoConferencia = empresaConfig.TipoConferencia.IdTipoConferencia.GetHashCode(),
                Referencia = produto.CodigoBarras,
                Multiplo = produto.MultiploVenda
                
            };

            return PartialView("EntradaConferencia", model);
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.TratarDivergencia)]
        public ActionResult TratarDivergencia(long id)
        {
            NotaFiscal notaFiscal = _uow.NotaFiscalRepository.GetById(id);

            var divergenciaViewModel = new RecebimentoTratarDivergenciaViewModel
            {
                ConferidoPor = "Usuário conferência",
                InicioConferencia = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"),
                FimConferencia = DateTime.Now.ToString("dd/MM/yyyy  hh:mm:ss"),
                NotaFiscal = notaFiscal.Numero.ToString(),
                IdNotaFiscal = notaFiscal.IdNotaFiscal,
                StatusNotasFiscal = notaFiscal.NotaFiscalStatus.Descricao,
            };

            List<LoteDivergencia> loteDivergencias = _uow.LoteDivergenciaRepository.RetornarPorNotaFiscal(id);

            foreach (LoteDivergencia divergencia in loteDivergencias)
            {
                var divergenciaItem = new RecebimentoTratarDivergenciaItemViewModel
                {
                    IdLoteDivergencia = divergencia.IdLoteDivergencia,
                    Referencia = divergencia.Produto.Referencia,
                    QuantidadeConferencia = divergencia.QuantidadeConferencia,
                    QuantidadeMais = divergencia.QuantidadeConferenciaMais ?? 0,
                    QuantidadeMenos = divergencia.QuantidadeConferenciaMenos ?? 0,
                    QuantidadeNotaFiscal = divergencia.NotaFiscal.NotaFiscalItens.Where(w => w.Produto.IdProduto == divergencia.Produto.IdProduto).First().Quantidade,
                    QuantidadePedido = 0
                };

                divergenciaViewModel.Divergencias.Add(divergenciaItem);
            }

            return View(divergenciaViewModel);
        }

        [HttpGet]
        public ActionResult RelatorioRastreioPeca()
        {
            return View(new RelatorioRastreioPecaViewModel());
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.RelatorioRastreioPeca)]
        public ActionResult RelatorioRastreioPecaPageData(DataTableFilter<RelatorioRastreioPecaFilterViewModel> model)
        {
            model.CustomFilter.IdEmpresa = IdEmpresa;

            var list = _uow.LoteConferenciaRepository.RastreioPeca(model.CustomFilter);

            int total = list.Count();

            var result = list.PaginationResult(model).Select(x => new RelatorioRastreioPecaListItemViewModel
            {
                DataRecebimento = x.DataRecebimento.ToString("dd/MM/yyyy"),
                Empresa = x.Empresa,
                IdEmpresa = x.IdEmpresa,
                IdLote = x.IdLote,
                NroNota = x.NroNota,
                QtdCompra = x.QtdCompra,
                QtdRecebida = x.QtdRecebida,
                ReferenciaPronduto = x.ReferenciaPronduto
            });

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = total,
                RecordsFiltered = total,
                Data = result
            });
        }

        [HttpPost]
        public ActionResult DownloadRelatorioRastreioPeca(RelatorioRastreioPecaRequest viewModel)
        {
            ValidateModel(viewModel);

            viewModel.IdEmpresa = IdEmpresa;
            viewModel.NomeUsuario = User.Identity.Name;

            byte[] relatorio = _relatorioService.GerarRelatorioRastreioPeca(viewModel);

            return File(relatorio, "application/pdf", "Relatório Rastreio de Peças.pdf");
        }
    }
}
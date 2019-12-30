using AutoMapper;
using ExtensionMethods.List;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Model.Etiquetas;
using FWLog.Services.Model.Lote;
using FWLog.Services.Model.Relatorios;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx;
using FWLog.Web.Backoffice.Models.CommonCtx;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class BORecebimentoNotaController : BOBaseController
    {
        private readonly RelatorioService _relatorioService;
        private readonly LoteService _loteService;
        private readonly ApplicationLogService _applicationLogService;
        private readonly EtiquetaService _etiquetaService;
        private readonly UnitOfWork _uow;

        public BORecebimentoNotaController(
            UnitOfWork uow,
            RelatorioService relatorioService,
            LoteService loteService,
            ApplicationLogService applicationLogService,
            EtiquetaService etiquetaService)
        {
            _loteService = loteService;
            _relatorioService = relatorioService;
            _applicationLogService = applicationLogService;
            _uow = uow;
            _etiquetaService = etiquetaService;
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

            model.Filter.IdStatus = LoteStatusEnum.AguardandoRecebimento.GetHashCode();
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

            var query = _uow.LoteRepository.Obter(IdEmpresa, NotaFiscalTipoEnum.Compra).AsQueryable();

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
                query = query.Where(x => x.QuantidadePeca == model.CustomFilter.QuantidadePeca);
            }

            if (model.CustomFilter.QuantidadeVolume.HasValue)
            {
                query = query.Where(x => x.QuantidadeVolume == model.CustomFilter.QuantidadeVolume);
            }

            if (model.CustomFilter.IdStatus.HasValue)
            {
                query = query.Where(x => (int)x.LoteStatus.IdLoteStatus == model.CustomFilter.IdStatus);
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
                        if (item.LoteStatus.IdLoteStatus == LoteStatusEnum.AguardandoRecebimento)
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
                        QuantidadePeca = item.QuantidadePeca == 0 ? (int?)null : item.QuantidadePeca,
                        QuantidadeVolume = item.QuantidadeVolume == 0 ? (int?)null : item.QuantidadeVolume,
                        RecebidoEm = item.LoteStatus.IdLoteStatus != LoteStatusEnum.AguardandoRecebimento ? item.DataRecebimento.ToString() : "-",
                        Status = item.LoteStatus.Descricao,
                        IdNotaFiscal = item.NotaFiscal.IdNotaFiscal,
                        Prazo = item.NotaFiscal.PrazoEntregaFornecedor.ToString("dd/MM/yyyy"),
                        Atraso = atraso,
                        IdUsuarioRecebimento = item.UsuarioRecebimento == null ? "" : item.UsuarioRecebimento.Id,
                        IdLoteStatus = (int)item.LoteStatus.IdLoteStatus
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
        public ActionResult ResumoFinalizarConferencia(long id)
        {
            ResumoFinalizarConferenciaResponse response = _loteService.ResumoFinalizarConferencia(id, IdEmpresa);

            var viewModel = Mapper.Map<ResumoFinalizarConferenciaViewModel>(response);

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult ResumoDivergenciaConferencia(long id)
        {
            var model = new ResumoDivergenciaConferenciaViewModel
            {
                IdNotaFiscal = _uow.LoteRepository.GetById(id).IdNotaFiscal,
                Divergencias = _loteService.ResumoFinalizarConferencia(id, IdEmpresa).Itens.Select(x => new ResumoDivergenciaConferenciaItemViewModel
                {
                    Referencia = x.Referencia,
                    QuantidadeConferencia = x.QuantidadeConferido,
                    QuantidadeNotaFiscal = x.QuantidadeNota,
                    QuantidadeMais = x.DivergenciaMais,
                    QuantidadeMenos = x.DivergenciaMenos
                }).ToList()
            };

            return View(model);
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

                var relatorioRequest = new ImprimirDetalhesNotaEntradaConferenciaRequest
                {
                    IdEmpresa = IdEmpresa,
                    NomeUsuario = User.Identity.Name,
                    IdNotaFiscal = viewModel.IdNotaFiscal
                };

                _relatorioService.ImprimirDetalhesNotaEntradaConferencia(relatorioRequest);

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
                FornecedorNome = string.Concat(notafiscal.Fornecedor.CodigoIntegracao.ToString(), " - ", notafiscal.Fornecedor.RazaoSocial),
                NumeroSerieNotaFiscal = string.Format("{0}-{1}", notafiscal.Numero, notafiscal.Serie),
                ValorTotal = notafiscal.ValorTotal.ToString("n2"),
                DataAtual = dataAtual,
                ValorFrete = notafiscal.ValorFrete.ToString("n2"),
                NumeroConhecimento = notafiscal.NumeroConhecimento,
                TransportadoraNome = string.Concat(notafiscal.Transportadora.CodigoIntegracao.ToString(), " - ", notafiscal.Transportadora.RazaoSocial),
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
                await _loteService.RegistrarRecebimentoNotaFiscal(idNotaFiscal, userInfo.UserId.ToString(), dataRecebimento, qtdVolumes).ConfigureAwait(false);
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

                var request = Mapper.Map<ImprimirRelatorioRecebimentoNotasRequest>(viewModel);
                _relatorioService.ImprimirRelatorioRecebimentoNotas(request);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Impressão enviada com sucesso."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Ocorreu um erro na impressão."
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ImprimirEtiquetaRecebimento(BOImprimirEtiquetaRecebimentoViewModel viewModel)
        {
            try
            {
                ValidateModel(viewModel);

                Lote lote = _uow.LoteRepository.ObterLoteNota(viewModel.IdNotaFiscal);
                _etiquetaService.ImprimirEtiquetaVolumeRecebimento(lote.IdLote, viewModel.IdImpressora);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Impressão realizada com sucesso."
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

        [HttpGet]
        public ActionResult DetalhesEntradaConferencia(long id)
        {
            NotaFiscal notaFiscal = _uow.NotaFiscalRepository.GetById(id);

            var model = new BODetalhesEntradaConferenciaViewModel
            {
                IdNotaFiscal = notaFiscal.IdNotaFiscal,
                ChaveAcesso = notaFiscal.ChaveAcesso,
                NumeroNotaFiscal = notaFiscal.Numero.ToString(),
                StatusNotaFiscal = notaFiscal.NotaFiscalStatus.ToString(),
                Fornecedor = string.Concat(notaFiscal.Fornecedor.CodigoIntegracao.ToString(), " - ", notaFiscal.Fornecedor.RazaoSocial),
                DataCompra = notaFiscal.DataEmissao.ToString("dd/MM/yyyy"),
                PrazoRecebimento = notaFiscal.PrazoEntregaFornecedor.ToString("dd/MM/yyyy"),
                FornecedorCNPJ = notaFiscal.Fornecedor.CNPJ.Substring(0, 2) + "." + notaFiscal.Fornecedor.CNPJ.Substring(2, 3) + "." + notaFiscal.Fornecedor.CNPJ.Substring(5, 3) + "/" + notaFiscal.Fornecedor.CNPJ.Substring(8, 4) + "-" + notaFiscal.Fornecedor.CNPJ.Substring(12, 2),
                ValorTotal = notaFiscal.ValorTotal.ToString("C"),
                ValorFrete = notaFiscal.ValorFrete.ToString("C"),
                NumeroConhecimento = notaFiscal.NumeroConhecimento.ToString(),
                PesoConhecimento = notaFiscal.PesoBruto.HasValue ? notaFiscal.PesoBruto.Value.ToString("F") : null,
                TransportadoraNome = string.Concat(notaFiscal.Transportadora.CodigoIntegracao.ToString(), " - ", notaFiscal.Transportadora.RazaoSocial),
                DiasAtraso = "0"
            };

            model.EmConferenciaOuConferido = false;

            if (notaFiscal.IdNotaFiscalStatus == NotaFiscalStatusEnum.AguardandoRecebimento || notaFiscal.IdNotaFiscalStatus == NotaFiscalStatusEnum.ProcessandoIntegracao)
            {
                model.StatusNotaFiscal = "Aguardando Recebimento";
                model.UsuarioRecebimento = "-";
                model.Volumes = notaFiscal.Quantidade.ToString();
                model.QuantidadePeca = notaFiscal.NotaFiscalItens.Sum(s => s.Quantidade).ToString();
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

                model.NumeroLote = lote.IdLote.ToString();
                model.DataChegada = lote.DataRecebimento.ToString("dd/MM/yyyy");
                model.UsuarioRecebimento = _uow.PerfilUsuarioRepository.GetByUserId(lote.UsuarioRecebimento.Id).Nome;
                model.Volumes = lote.QuantidadeVolume.ToString();
                model.QuantidadePeca = lote.QuantidadePeca.ToString();
                model.StatusNotaFiscal = notaFiscal.NotaFiscalStatus.Descricao;

                if (lote.DataRecebimento > notaFiscal.PrazoEntregaFornecedor)
                {
                    TimeSpan atraso = lote.DataRecebimento.Subtract(notaFiscal.PrazoEntregaFornecedor);
                    model.DiasAtraso = atraso.Days.ToString();
                }

                #region CONFERENCIA
                //Verifica se o lote está em conferência ou já foi conferido.
                if (lote.IdLoteStatus == LoteStatusEnum.Conferencia ||
                    lote.IdLoteStatus == LoteStatusEnum.ConferidoDivergencia ||
                    lote.IdLoteStatus == LoteStatusEnum.Finalizado ||
                    lote.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaNegativa ||
                    lote.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaPositiva ||
                    lote.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaTodas)
                {
                    model.EmConferenciaOuConferido = true;

                    //Captura as conferências do lote.
                    var loteConferencia = _uow.LoteConferenciaRepository.ObterPorId(lote.IdLote);

                    if (loteConferencia.Count > 0)
                    {
                        model.ConferenciaTipo = loteConferencia.FirstOrDefault().TipoConferencia.Descricao;

                        //Captura o primeiro conferente.
                        model.UsuarioConferencia = _uow.PerfilUsuarioRepository.GetByUserId(loteConferencia.FirstOrDefault().UsuarioConferente.Id).Nome;

                        //Captura a menor data de início da conferência.
                        model.DataInicioConferencia = loteConferencia.Min(x => x.DataHoraInicio).ToString("dd/MM/yyyy HH:mm");

                        //Captura a maior data fim de conferência.
                        if (lote.IdLoteStatus == LoteStatusEnum.ConferidoDivergencia ||
                            lote.IdLoteStatus == LoteStatusEnum.Finalizado ||
                            lote.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaNegativa ||
                            lote.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaPositiva ||
                            lote.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaTodas)
                            model.DataFimConferencia = loteConferencia.Max(x => x.DataHoraFim).ToString("dd/MM/yyyy HH:mm:ss");

                        var tempo = new TimeSpan(0, 0, 0);

                        List<UsuarioEmpresa> usuarios = _uow.UsuarioEmpresaRepository.ObterPorEmpresa(IdEmpresa);

                        //Calcula o tempo total.
                        foreach (var item in loteConferencia)
                        {
                            tempo.Add(new TimeSpan(item.Tempo.Hour, item.Tempo.Minute, item.Tempo.Second));

                            var entradaConferenciaItem = new BODetalhesEntradaConferenciaItem
                            {
                                Referencia = item.Produto.Referencia,
                                Quantidade = item.Quantidade,
                                DataInicioConferencia = item.DataHoraInicio.ToString("dd/MM/yyyy HH:mm:ss"),
                                DataFimConferencia = item.DataHoraFim.ToString("dd/MM/yyyy HH:mm:ss"),
                                TempoConferencia = item.Tempo.ToString("HH:mm:ss"),
                                UsuarioConferencia = usuarios.Where(x => x.UserId.Equals(item.UsuarioConferente.Id)).FirstOrDefault()?.PerfilUsuario.Nome
                            };

                            model.Items.Add(entradaConferenciaItem);
                        }

                        if (lote.IdLoteStatus == LoteStatusEnum.ConferidoDivergencia ||
                            lote.IdLoteStatus == LoteStatusEnum.Finalizado ||
                            lote.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaNegativa ||
                            lote.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaPositiva ||
                            lote.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaTodas)
                            model.TempoTotalConferencia = tempo.ToString("h'h 'm'm 's's'");
                    }

                    var _emConferencia = new[] { LoteStatusEnum.Conferencia, LoteStatusEnum.ConferidoDivergencia };
                    if (!Array.Exists(_emConferencia, x => x == lote.IdLoteStatus))
                    {
                        model.Finalizado = true;

                        List<LoteDivergencia> loteDivergencias = _uow.LoteDivergenciaRepository.RetornarPorNotaFiscal(notaFiscal.IdNotaFiscal);

                        if (loteDivergencias.Any())
                        {
                            PerfilUsuario perfilUsuario = _uow.PerfilUsuarioRepository.GetByUserId(loteConferencia.First().IdUsuarioConferente);

                            var divergenciaViewModel = new ExibirDivergenciaRecebimentoViewModel
                            {
                                ConferidoPor = perfilUsuario.Nome,
                                InicioConferencia = loteConferencia.First().DataHoraInicio.ToString("dd/MM/yyyy hh:mm:ss"),
                                FimConferencia = loteConferencia.Last().DataHoraFim.ToString("dd/MM/yyyy hh:mm:ss"),
                                NotaFiscal = notaFiscal.Numero.ToString(),
                                IdLote = lote.IdLote,
                                StatusNotasFiscal = notaFiscal.NotaFiscalStatus.Descricao,
                                UsuarioTratamento = _uow.PerfilUsuarioRepository.GetByUserId(loteDivergencias.First().IdUsuarioDivergencia).Nome,
                                DataTratamento = loteDivergencias.First().DataTratamentoDivergencia.Value.ToString("dd/MM/yyyy hh:mm:ss")
                            };

                            foreach (LoteDivergencia divergencia in loteDivergencias)
                            {
                                NotaFiscalItem nfItem = divergencia.NotaFiscal.NotaFiscalItens.Where(w => w.Produto.IdProduto == divergencia.Produto.IdProduto).FirstOrDefault();

                                var divergenciaItem = new ExibirDivergenciaRecebimentoItemViewModel
                                {
                                    Referencia = divergencia.Produto.Referencia,
                                    QuantidadeConferencia = divergencia.QuantidadeConferencia,
                                    QuantidadeMais = divergencia.QuantidadeConferenciaMais ?? 0,
                                    QuantidadeMenos = divergencia.QuantidadeConferenciaMenos ?? 0,
                                    QuantidadeNotaFiscal = nfItem == null ? 0 : nfItem.Quantidade,
                                    QuantidadeMaisTratado = divergencia.QuantidadeDivergenciaMais ?? 0,
                                    QuantidadeMenosTratado = divergencia.QuantidadeDivergenciaMenos ?? 0
                                };

                                divergenciaViewModel.Divergencias.Add(divergenciaItem);
                            }

                            model.Divergencias = divergenciaViewModel;
                        }
                    }
                }
                #endregion
            }

            return View(model);
        }

        public JsonResult ValidarInicioConferencia(long id)
        {
            NotaFiscal notaFiscal = _uow.NotaFiscalRepository.GetById(id);

            //Valida a Nota Fiscal.
            if (notaFiscal == null)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Não foi possível buscar a Nota Fiscal. Por favor, tente novamente!"
                });
            }

            var lote = _uow.LoteRepository.PesquisarLotePorNotaFiscal(id);

            //Valida o Lote.
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
                if (lote.IdLoteStatus == LoteStatusEnum.ConferidoDivergencia)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "O Lote já foi conferido."
                    });
                }
                else if (
                    lote.IdLoteStatus == LoteStatusEnum.Finalizado ||
                    lote.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaNegativa ||
                    lote.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaPositiva ||
                    lote.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaTodas
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


        [HttpGet]
        public ActionResult EntradaConferencia(long id)
        {
            var lote = _uow.LoteRepository.PesquisarLotePorNotaFiscal(id);

            //Valida o Lote.
            if (lote == null)
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Lote não encontrado. Por favor, tente novamente!");

            var usuarioLogado = new BackOfficeUserInfo();

            //Captura o Usuário que está iniciando a conferência.
            var usuario = _uow.PerfilUsuarioRepository.GetByUserId(User.Identity.GetUserId());

            var empresaConfig = _uow.EmpresaConfigRepository.ConsultarPorIdEmpresa(IdEmpresa);

            if (empresaConfig == null)
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "As configurações da empresa não foi encontrada. Por favor, tente novamente!");

            var model = new BOEntradaConferenciaViewModel
            {
                IdNotaFiscal = lote.NotaFiscal.IdNotaFiscal,
                IdLote = lote.IdLote,
                NumeroNotaFiscal = string.Concat(lote.NotaFiscal.Numero, " - ", lote.NotaFiscal.Serie),
                IdUuarioConferente = usuario.UsuarioId,
                NomeConferente = usuario.Nome,
                DataHoraRecebimento = lote.DataRecebimento.ToString("dd/MM/yyyy HH:mm"),
                NomeFornecedor = lote.NotaFiscal.Fornecedor.NomeFantasia,
                QuantidadeVolume = lote.QuantidadeVolume,
                TipoConferencia = empresaConfig.TipoConferencia.Descricao,
                IdTipoConferencia = empresaConfig.TipoConferencia.IdTipoConferencia.GetHashCode()
            };

            //Se o tipo da conferência for, o usuário não poderá informar a quantidade por caixa e quantidade de caixa.
            //Sabendo disso, atribui 1 para os campos.
            if (empresaConfig.TipoConferencia.IdTipoConferencia == TipoConferenciaEnum.ConferenciaCemPorcento)
            {
                model.QuantidadePorCaixa = 1;
                model.QuantidadeCaixa = 1;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult ObterDadosReferenciaConferencia(string codigoBarrasOuReferencia, long idLote)
        {
            //Valida se o código de barras ou referência é vazio ou nulo.
            if (string.IsNullOrEmpty(codigoBarrasOuReferencia))
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Referência inválida. Por favor, tente novamente!"
                });
            }

            var produto = _uow.ProdutoRepository.ConsultarPorCodigoBarrasOuReferencia(codigoBarrasOuReferencia);

            //Valida se foi encontrado um produto através do código de barras ou da referência.
            if (produto == null)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Referência não cadastrada. Por favor, tente novamente!"
                });
            }

            //Captura o lote novamente.
            var lote = _uow.LoteRepository.GetById(idLote);

            if (lote == null)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Lote não encontrado. Por favor, tente novamente!"
                });
            }

            var usuarioLogado = new BackOfficeUserInfo();

            //Captura o Usuário que está iniciando a conferência novamente.
            var usuario = _uow.PerfilUsuarioRepository.GetByUserId(User.Identity.GetUserId());

            var empresaConfig = _uow.EmpresaConfigRepository.ConsultarPorIdEmpresa(IdEmpresa);

            if (empresaConfig == null)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "As configurações da empresa não foi encontrada. Por favor, tente novamente!"
                });
            }

            //Captura a quantidade do item (peça) da nota e da conferência.
            var referenciaNota = _uow.NotaFiscalItemRepository.ObterPorItem(lote.IdNotaFiscal, produto.IdProduto);
            var referenciaConferencia = _uow.LoteConferenciaRepository.ObterPorProduto(idLote, produto.IdProduto);
            ProdutoEstoque empresaProduto = _uow.ProdutoEstoqueRepository.ObterPorProdutoEmpresa(produto.IdProduto, IdEmpresa);

            int quantidadeNota = 0;
            int quantidadeConferida = 0;
            int quantidadeNaoConferida = 0;

            //Caso o item exista na nota, captura a quantidade.
            if (referenciaNota.Count > 0)
                quantidadeNota = referenciaNota.Sum(x => x.Quantidade);

            //Caso exista o item já tenha sido conferido, captura a quantidade.
            if (referenciaConferencia.Count > 0)
                quantidadeConferida = referenciaConferencia.Sum(x => x.Quantidade);

            if (quantidadeNota > quantidadeConferida)
                quantidadeNaoConferida = quantidadeNota - quantidadeConferida;

            var model = new BOEntradaConferenciaViewModel
            {
                IdNotaFiscal = lote.NotaFiscal.IdNotaFiscal,
                IdLote = lote.IdLote,
                NumeroNotaFiscal = lote.NotaFiscal.Numero + lote.NotaFiscal.Serie,
                IdUuarioConferente = usuario.UsuarioId,
                NomeConferente = usuario.Nome,
                DataHoraRecebimento = lote.DataRecebimento.ToString("dd/MM/yyyy HH:mm"),
                NomeFornecedor = lote.NotaFiscal.Fornecedor.NomeFantasia,
                QuantidadeVolume = lote.QuantidadeVolume,
                TipoConferencia = empresaConfig.TipoConferencia.Descricao,
                IdTipoConferencia = empresaConfig.TipoConferencia.IdTipoConferencia.GetHashCode(),
                Referencia = produto.Referencia,
                DescricaoReferencia = produto.Descricao,
                Embalagem = "",
                Unidade = "",
                Multiplo = produto.MultiploVenda,
                QuantidadeEstoque = empresaProduto == null ? 0 : empresaProduto.Saldo,
                QuantidadeNaoConferida = quantidadeNaoConferida,
                QuantidadeConferida = quantidadeConferida,
                InicioConferencia = DateTime.Now.ToString()
            };

            if (empresaProduto == null || (empresaProduto != null && empresaProduto.EnderecoArmazenagem == null))
            {
                model.Localizacao = string.Empty;
                model.EnviarPicking = true;
            }
            else
            {
                model.Localizacao = empresaProduto.EnderecoArmazenagem.Codigo;
                model.EnviarPicking = empresaProduto.Saldo == 0 ? true : false;
            }

            string json = JsonConvert.SerializeObject(model);

            return Json(new AjaxGenericResultModel
            {
                Success = true,
                Message = "",
                Data = json
            });
        }

        public JsonResult RegistrarConferencia(string codigoBarrasOuReferencia, long idLote, int quantidadePorCaixa, int quantidadeCaixa, string inicioConferencia)
        {
            try
            {
                //Valida novamente se a referência é valida.
                if (string.IsNullOrEmpty(codigoBarrasOuReferencia))
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Referência inválida. Por favor, tente novamente!"
                    });
                }

                var produto = _uow.ProdutoRepository.ConsultarPorCodigoBarrasOuReferencia(codigoBarrasOuReferencia);

                //Valida se o produto (peça) foi encontrado.
                if (produto == null)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Referência não cadastrada. Por favor, tente novamente!"
                    });
                }

                var lote = _uow.LoteRepository.GetById(idLote);

                //Valida o lote.
                if (lote == null)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Lote não encontrado. Por favor, tente novamente!"
                    });
                }

                var usuarioLogado = new BackOfficeUserInfo();

                //Captura o usuário novamente.
                var usuario = _uow.PerfilUsuarioRepository.GetByUserId(User.Identity.GetUserId());

                var empresaConfig = _uow.EmpresaConfigRepository.ConsultarPorIdEmpresa(IdEmpresa);

                //Valida as configurações da empresa.
                if (empresaConfig == null)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "As configurações da empresa não foi encontrada. Por favor, tente novamente!"
                    });
                }

                //Valida se os campos quantidade por caixa e quantidade de caixa são iguais a 1 quando o tipo da conferência é 100%.
                //Isso é feito porque na conferência 100% a quantidade por caixa e quantidade de caixa não podem ser maior que 1.
                if (empresaConfig.TipoConferencia.IdTipoConferencia == TipoConferenciaEnum.ConferenciaCemPorcento
                    && quantidadePorCaixa != 1 && quantidadeCaixa != 1)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Neste tipo de conferência, não é permitido um valor diferente de 1 nos campos quantidade por caixa e quantidade de caixa. Por favor, tente novamente!"
                    });
                }

                //Valida se a quantidade por caixa e quantidade de caixa é igual a 0.
                if (quantidadePorCaixa == 0 || (quantidadePorCaixa > 0 && quantidadeCaixa == 0))
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Os campos quantidade por caixa e quantidade de caixa não podem ser 0. Por favor, tente novamente!"
                    });
                }

                if (quantidadePorCaixa < 0)
                {
                    var totalConferido = _uow.LoteConferenciaRepository.ObterPorProduto(lote.IdLote, produto.IdProduto).Sum(x => x.Quantidade);

                    if (totalConferido - quantidadePorCaixa * -1 < 0)
                    {
                        return Json(new AjaxGenericResultModel
                        {
                            Success = false,
                            Message = "A quantidade por caixa não pode ser menor que a quantidade conferida. Por favor, tente novamente!"
                        });
                    }

                }

                //Decidi não verificar o status do lote, sendo assim, sempre atualizado para Em Conferência.
                //Validações anteriores garantem que o status não será atualizado se diferente de Recebido.
                lote.IdLoteStatus = LoteStatusEnum.Conferencia;
                _uow.LoteRepository.Update(lote);

                DateTime dataHoraInicio = !string.IsNullOrEmpty(inicioConferencia) ? Convert.ToDateTime(inicioConferencia) : DateTime.Now;
                DateTime dataHoraFim = DateTime.Now;

                TimeSpan diferenca = dataHoraFim - dataHoraInicio;

                DateTime tempo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, diferenca.Hours, diferenca.Minutes, diferenca.Seconds);

                LoteConferencia loteConferencia = new LoteConferencia()
                {
                    IdLote = lote.IdLote,
                    IdTipoConferencia = empresaConfig.IdTipoConferencia.Value,
                    IdProduto = produto.IdProduto,
                    Quantidade = quantidadePorCaixa > 0 ? quantidadePorCaixa * quantidadeCaixa : quantidadePorCaixa,
                    DataHoraInicio = dataHoraInicio,
                    DataHoraFim = dataHoraFim,
                    Tempo = tempo,
                    IdUsuarioConferente = usuario.UsuarioId
                };

                _uow.LoteConferenciaRepository.Add(loteConferencia);

                _uow.SaveChanges();

                #region AGUARDANDO A DEFINIÇÃO DE IMPRESSORA PADRÃO

                //var request = new EtiquetaArmazenagemVolumeRequest
                //{
                //    NroLote = idLote,
                //    QuantidadeEtiquetas = quantidadeCaixa,
                //    QuantidadePorCaixa = quantidadePorCaixa,
                //    ReferenciaProduto = produto.Referencia,
                //    Usuario = _uow.PerfilUsuarioRepository.GetByUserId(User.Identity.GetUserId())?.Nome,
                //    IdImpressora = null
                //};

                //_etiquetaService.ImprimirEtiquetaArmazenagemVolume(request);

                #endregion

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Conferência registrada com sucesso!"
                });
            }
            catch (Exception e)
            {
                _applicationLogService.Error(ApplicationEnum.BackOffice, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Não foi possível registrar a conferência. Por favor, tente novamente!"
                });
            }
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.TratarDivergencia)]
        public ActionResult TratarDivergencia(long id)
        {
            NotaFiscal notaFiscal = _uow.NotaFiscalRepository.GetById(id);
            Lote lote = _uow.LoteRepository.ObterLoteNota(id);
            List<LoteConferencia> loteConferencia = _uow.LoteConferenciaRepository.Obter(lote.IdLote);

            PerfilUsuario perfilUsuario = _uow.PerfilUsuarioRepository.GetByUserId(loteConferencia.First().IdUsuarioConferente);

            var divergenciaViewModel = new TratarDivergenciaRecebimentoViewModel
            {
                ConferidoPor = perfilUsuario.Nome,
                NotaFiscal = notaFiscal.Numero.ToString(),
                IdNotaFiscal = notaFiscal.IdNotaFiscal,
                StatusNotasFiscal = notaFiscal.NotaFiscalStatus.Descricao
            };

            List<LoteDivergencia> loteDivergencias = _uow.LoteDivergenciaRepository.RetornarPorNotaFiscal(id);

            if (loteDivergencias.Count > 0)
            {
                divergenciaViewModel.InicioConferencia = loteConferencia.First().DataHoraInicio.ToString("dd/MM/yyyy hh:mm:ss");
                divergenciaViewModel.FimConferencia = loteConferencia.Last().DataHoraFim.ToString("dd/MM/yyyy hh:mm:ss");
            }

            foreach (LoteDivergencia divergencia in loteDivergencias)
            {
                NotaFiscalItem nfItem = divergencia.NotaFiscal.NotaFiscalItens.Where(w => w.Produto.IdProduto == divergencia.Produto.IdProduto).FirstOrDefault();

                var divergenciaItem = new TratarDivergenciaRecebimentoItemViewModel
                {
                    IdLoteDivergencia = divergencia.IdLoteDivergencia,
                    Referencia = divergencia.Produto.Referencia,
                    QuantidadeConferencia = divergencia.QuantidadeConferencia,
                    QuantidadeMais = divergencia.QuantidadeConferenciaMais ?? 0,
                    QuantidadeMenos = divergencia.QuantidadeConferenciaMenos ?? 0,
                    QuantidadeNotaFiscal = nfItem == null ? 0 : nfItem.Quantidade
                };

                divergenciaViewModel.Divergencias.Add(divergenciaItem);
            }

            return View(divergenciaViewModel);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.TratarDivergencia)]
        public async Task<JsonResult> TratarDivergencia(TratarDivergenciaRecebimentoViewModel viewModel)
        {
            try
            {
                ValidateModel(viewModel);
                var request = Mapper.Map<TratarDivergenciaRequest>(viewModel);
                request.IdUsuario = User.Identity.GetUserId();
                request.IdEmpresa = IdEmpresa;

                await _loteService.TratarDivergencia(request).ConfigureAwait(false);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Tratamento de divergências finalizado."
                }, JsonRequestBehavior.DenyGet);

            }
            catch (Exception e)
            {
                _applicationLogService.Error(ApplicationEnum.BackOffice, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = e is BusinessException ? e.Message : "Não foi possível comunicar a finalização da tratativa de divergência com o Sankhya."
                }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.TratarDivergencia)]
        public ActionResult ExibirDivergencia(long id)
        {
            NotaFiscal notaFiscal = _uow.NotaFiscalRepository.GetById(id);
            Lote lote = _uow.LoteRepository.ObterLoteNota(id);
            List<LoteConferencia> loteConferencia = _uow.LoteConferenciaRepository.Obter(lote.IdLote);

            PerfilUsuario perfilUsuario = _uow.PerfilUsuarioRepository.GetByUserId(loteConferencia.First().IdUsuarioConferente);

            List<LoteDivergencia> loteDivergencias = _uow.LoteDivergenciaRepository.RetornarPorNotaFiscal(id);

            var divergenciaViewModel = new ExibirDivergenciaRecebimentoViewModel
            {
                ConferidoPor = perfilUsuario.Nome,
                InicioConferencia = loteConferencia.First().DataHoraInicio.ToString("dd/MM/yyyy hh:mm:ss"),
                FimConferencia = loteConferencia.Last().DataHoraFim.ToString("dd/MM/yyyy hh:mm:ss"),
                NotaFiscal = notaFiscal.Numero.ToString(),
                IdLote = lote.IdLote,
                StatusNotasFiscal = notaFiscal.NotaFiscalStatus.Descricao,
                UsuarioTratamento = _uow.PerfilUsuarioRepository.GetByUserId(loteDivergencias.First().IdUsuarioDivergencia).Nome,
                DataTratamento = loteDivergencias.First().DataTratamentoDivergencia.Value.ToString("dd/MM/yyyy hh:mm:ss")
            };

            foreach (LoteDivergencia divergencia in loteDivergencias)
            {
                NotaFiscalItem nfItem = divergencia.NotaFiscal.NotaFiscalItens.Where(w => w.Produto.IdProduto == divergencia.Produto.IdProduto).FirstOrDefault();

                var divergenciaItem = new ExibirDivergenciaRecebimentoItemViewModel
                {
                    Referencia = divergencia.Produto.Referencia,
                    QuantidadeConferencia = divergencia.QuantidadeConferencia,
                    QuantidadeMais = divergencia.QuantidadeConferenciaMais ?? 0,
                    QuantidadeMenos = divergencia.QuantidadeConferenciaMenos ?? 0,
                    QuantidadeNotaFiscal = nfItem == null ? 0 : nfItem.Quantidade,
                    QuantidadeMaisTratado = divergencia.QuantidadeDivergenciaMais ?? 0,
                    QuantidadeMenosTratado = divergencia.QuantidadeDivergenciaMenos ?? 0
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

        [HttpPost]
        public async Task<JsonResult> FinalizarConferencia(long id)
        {
            try
            {
                await _loteService.FinalizarConferencia(id, User.Identity.GetUserId(), IdEmpresa).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _applicationLogService.Error(ApplicationEnum.BackOffice, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = e is BusinessException ? e.Message : "Não foi possível comunicar a finalização da conferência com o Sankhya."
                });
            }

            return Json(new AjaxGenericResultModel
            {
                Success = true,
                Message = "Finalização da conferência realizada com sucesso."
            });
        }

        [HttpGet]
        public ActionResult RelatorioResumoProducao()
        {
            return View(new RelatorioResumoProducaoViewModel());
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.ListarResumoProducao)]
        public ActionResult ResumoProducaoRecebimentoPageData(DataTableFilter<RelatorioResumoProducaoFilterViewModel> model)
        {
            var filter = new RelatorioResumoProducaoFilter
            {
                DateMin = model.CustomFilter.DataRecebimentoMinima,
                DateMax = model.CustomFilter.DataRecebimentoMaxima,
                UserId = model.CustomFilter.IdUsuario,
                IdEmpresa = IdEmpresa
            };

            int total = _uow.LoteRepository.Todos().Select(x => x.IdUsuarioRecebimento).Distinct().Count();

            var list = _uow.LoteRepository.ResumoProducaoRecebimento(filter).Select(x => new RelatorioResumoProducaoRecebimentoListItemViewModel
            {
                NomeUsuario = x.Nome,
                NotasRecebidas = x.NOTASRECEBIDAS,
                NotasRecebidasUsuario = x.NOTASRECEBIDASUSUARIO,
                VolumesRecebidos = x.VOLUMESRECEBIDOS,
                VolumesRecebidosUsuario = x.VOLUMESRECEBIDOSUSUARIO,
                Percentual = x.PERCENTUAL,
                Ranking = x.RANKING
            });

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = total,
                RecordsFiltered = list.Count(),
                Data = list
            });
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.ListarResumoProducao)]
        public ActionResult ResumoProducaoConferenciaPageData(DataTableFilter<RelatorioResumoProducaoFilterViewModel> model)
        {
            var filter = new RelatorioResumoProducaoFilter
            {
                DateMin = model.CustomFilter.DataRecebimentoMinima,
                DateMax = model.CustomFilter.DataRecebimentoMaxima,
                UserId = model.CustomFilter.IdUsuario,
                IdEmpresa = IdEmpresa
            };

            int total = _uow.LoteConferenciaRepository.Todos().Select(x => x.IdUsuarioConferente).Distinct().Count();

            var list = _uow.LoteConferenciaRepository.ResumoProducaoConferencia(filter).Select(x => new RelatorioResumoProducaoConferenciaListItemViewModel
            {
                NomeUsuario = x.Nome,
                LotesRecebidos = x.LOTESRECEBIDOS,
                LotesRecebidosUsuario = x.LOTESRECEBIDASUSUARIO,
                PecasRecebidas = x.PECASRECEBIDAS,
                PecasRecebidasUsuario = x.PECASRECEBIDASUSUARIO,
                Percentual = x.PERCENTUAL,
                Ranking = x.RANKING
            });

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = total,
                RecordsFiltered = list.Count(),
                Data = list
            });
        }

    }
}
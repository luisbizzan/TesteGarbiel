using AutoMapper;
using ExtensionMethods.List;
using ExtensionMethods.String;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
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
using System.Text.RegularExpressions;
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
        private readonly LogEtiquetagemService _logEtiquetagemService;
        private readonly NotaFiscalService _notaFiscalService;
        private readonly ProdutoService _produtoService;
        private readonly ConferenciaService _conferenciaService;
        private readonly UnitOfWork _uow;

        public BORecebimentoNotaController(
            UnitOfWork uow,
            RelatorioService relatorioService,
            LoteService loteService,
            ApplicationLogService applicationLogService,
            EtiquetaService etiquetaService,
            LogEtiquetagemService logEtiquetagemService,
            NotaFiscalService notaFiscalService,
            ConferenciaService conferenciaService,
            ProdutoService produtoService)
        {
            _loteService = loteService;
            _relatorioService = relatorioService;
            _applicationLogService = applicationLogService;
            _uow = uow;
            _etiquetaService = etiquetaService;
            _logEtiquetagemService = logEtiquetagemService;
            _notaFiscalService = notaFiscalService;
            _conferenciaService = conferenciaService;
            _produtoService = produtoService;
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.List)]
        public ActionResult Index()
        {
            var model = new BORecebimentoNotaListViewModel
            {
                Filter = new BORecebimentoNotaFilterViewModel()
                {
                    ListaStatus = new SelectList(
                    _uow.LoteStatusRepository.Todos().OrderBy(o => o.IdLoteStatus).Select(x => new SelectListItem
                    {
                        Value = x.IdLoteStatus.GetHashCode().ToString(),
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
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.List)]
        public ActionResult PageData(DataTableFilter<BORecebimentoNotaFilterViewModel> model)
        {
            List<BORecebimentoNotaListItemViewModel> boRecebimentoNotaListItemViewModel = new List<BORecebimentoNotaListItemViewModel>();
            int totalRecords = 0;
            int totalRecordsFiltered = 0;
            if (model.CustomFilter.PrazoInicial == null)
            {
                if (model.CustomFilter.DataInicial == null)
                    ModelState.AddModelError(nameof(model.CustomFilter.PrazoInicial), "Prazo inicial obrigatório.");
            }

            if (model.CustomFilter.PrazoFinal == null)
            {
                if (model.CustomFilter.DataFinal == null)
                    ModelState.AddModelError(nameof(model.CustomFilter.PrazoFinal), "Prazo final obrigatório.");
            }

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

            var query = _uow.LoteRepository.Obter(IdEmpresa, NotaFiscalTipoEnum.Compra);

            totalRecords = query.Count();

            if (!string.IsNullOrEmpty(model.CustomFilter.ChaveAcesso))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.NotaFiscal.ChaveAcesso) && x.NotaFiscal.ChaveAcesso.Contains(model.CustomFilter.ChaveAcesso));
            }

            if (model.CustomFilter.Lote.HasValue)
            {
                query = query.Where(x => x.IdLote == model.CustomFilter.Lote.Value);
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
                DateTime prazoInicial = new DateTime(model.CustomFilter.PrazoInicial.Value.Year, model.CustomFilter.PrazoInicial.Value.Month, model.CustomFilter.PrazoInicial.Value.Day,
                    00, 00, 00);
                query = query.Where(x => x.NotaFiscal.PrazoEntregaFornecedor >= prazoInicial);
            }

            if (model.CustomFilter.PrazoFinal != null)
            {
                DateTime prazoFinal = new DateTime(model.CustomFilter.PrazoFinal.Value.Year, model.CustomFilter.PrazoFinal.Value.Month, model.CustomFilter.PrazoFinal.Value.Day,
                    23, 59, 59);
                query = query.Where(x => x.NotaFiscal.PrazoEntregaFornecedor <= prazoFinal);
            }

            if (!model.CustomFilter.IdUsuarioConferencia.NullOrEmpty())
            {
                var lotes = query.Select(s => s.IdLote).ToList();
                var conferencias = _uow.LoteConferenciaRepository.Todos().Where(w => w.IdUsuarioConferente == model.CustomFilter.IdUsuarioConferencia && lotes.Contains(w.IdLote)).Select(s => s.IdLote).ToList();
                query = query.Where(x => conferencias.Contains(x.IdLote));
            }

            if (!model.CustomFilter.IdUsuarioRecebimento.NullOrEmpty())
            {
                query = query.Where(x => x.UsuarioRecebimento?.Id == model.CustomFilter.IdUsuarioRecebimento);
            }

            if (!string.IsNullOrEmpty(model.CustomFilter.TempoInicial))
            {
                long hora = Convert.ToInt32(model.CustomFilter.TempoInicial.Substring(0, 2));
                long minutos = Convert.ToInt32(model.CustomFilter.TempoInicial.Substring(3, 2));
                long totalSegundos = (hora * 3600) + (minutos * 60);

                query = query.Where(x => x.TempoTotalConferencia >= totalSegundos);
            }

            if (!string.IsNullOrEmpty(model.CustomFilter.TempoFinal))
            {
                long hora = Convert.ToInt32(model.CustomFilter.TempoFinal.Substring(0, 2));
                long minutos = Convert.ToInt32(model.CustomFilter.TempoFinal.Substring(3, 2));
                long totalSegundos = (hora * 3600) + (minutos * 60);

                query = query.Where(x => x.TempoTotalConferencia <= totalSegundos);
            }

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

                if (model.CustomFilter.Atraso.HasValue && model.CustomFilter.Atraso.Value != atraso)
                {
                    continue;
                }

                var empresaConfig = _uow.EmpresaConfigRepository.ConsultarPorIdEmpresa(IdEmpresa);

                boRecebimentoNotaListItemViewModel.Add(new BORecebimentoNotaListItemViewModel()
                {
                    Lote = item.IdLote == 0 ? (long?)null : item.IdLote,
                    Nota = item.NotaFiscal.Numero == 0 ? (long?)null : item.NotaFiscal.Numero,
                    Fornecedor = item.NotaFiscal.Fornecedor.NomeFantasia,
                    QuantidadePeca = item.QuantidadePeca == 0 ? (int?)null : item.QuantidadePeca,
                    QuantidadeVolume = item.QuantidadeVolume == 0 ? (int?)null : item.QuantidadeVolume,
                    RecebidoEm = item.LoteStatus.IdLoteStatus != LoteStatusEnum.AguardandoRecebimento ? item.DataRecebimento.ToString("dd/MM/yyyy HH:mm") : " - ",
                    Status = item.LoteStatus.Descricao,
                    IdNotaFiscal = item.NotaFiscal.IdNotaFiscal,
                    Prazo = item.NotaFiscal.PrazoEntregaFornecedor.ToString("dd/MM/yyyy"),
                    Atraso = atraso,
                    IdUsuarioRecebimento = item.UsuarioRecebimento == null ? string.Empty : item.UsuarioRecebimento.Id,
                    IdLoteStatus = (int)item.LoteStatus.IdLoteStatus,
                    ConferenciaAutomatica = empresaConfig.CNPJConferenciaAutomatica == item.NotaFiscal.Fornecedor.CNPJ,
                    DataVencimento = item.NotaFiscal.DataVencimento.HasValue ? item.NotaFiscal.DataVencimento.Value.ToString("dd/MM/yyyy") : ""
                });
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
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.ConferirLote)]
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
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.ConferirLote)]
        public ActionResult ResumoFinalizarConferencia(long id)
        {
            ResumoFinalizarConferenciaResponse response = _loteService.ResumoFinalizarConferencia(id, IdEmpresa, User.Identity.GetUserId());

            var viewModel = Mapper.Map<ResumoFinalizarConferenciaViewModel>(response);

            return View(viewModel);
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.ConferirLote)]
        public ActionResult ResumoDivergenciaConferencia(long id)
        {
            var lote = _uow.LoteRepository.GetById(id);
            var usuario = _uow.PerfilUsuarioRepository.GetByUserId(User.Identity.GetUserId());
            var empresaConfig = _uow.EmpresaConfigRepository.ConsultarPorIdEmpresa(IdEmpresa);

            var model = new ResumoDivergenciaConferenciaViewModel
            {
                IdNotaFiscal = lote.IdNotaFiscal,
                IdLote = lote.IdLote,
                NumeroNotaFiscal = string.Concat(lote.NotaFiscal.Numero, " - ", lote.NotaFiscal.Serie),
                NomeConferente = usuario.Nome,
                DataHoraRecebimento = lote.DataRecebimento.ToString("dd/MM/yyyy HH:mm"),
                NomeFornecedor = lote.NotaFiscal.Fornecedor.NomeFantasia,
                QuantidadeVolume = lote.QuantidadeVolume,
                TipoConferencia = empresaConfig.TipoConferencia.Descricao,

                Divergencias = _loteService.ResumoFinalizarConferencia(id, IdEmpresa, User.Identity.GetUserId()).Itens.Where(x => x.DivergenciaMais > 0 || x.DivergenciaMenos > 0).Select(x => new ResumoDivergenciaConferenciaItemViewModel
                {
                    Referencia = x.Referencia,
                    DescricaoProduto = x.DescricaoProduto,
                    QuantidadeConferencia = x.QuantidadeConferido,
                    QuantidadeNotaFiscal = x.QuantidadeNota,
                    QuantidadeMais = x.DivergenciaMais,
                    QuantidadeMenos = x.DivergenciaMenos
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.List)]
        public ActionResult DownloadRelatorioNotas(BODownloadRelatorioNotasViewModel viewModel)
        {
            ValidateModel(viewModel);

            var relatorioRequest = Mapper.Map<RelatorioRecebimentoNotasRequest>(viewModel);
            relatorioRequest.IdEmpresa = IdEmpresa;
            relatorioRequest.NomeUsuario = LabelUsuario;
            byte[] relatorio = _relatorioService.GerarRelatorioRecebimentoNotas(relatorioRequest);

            return File(relatorio, "application/pdf", "Relatório Recebimento Notas.pdf");
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.List)]
        public ActionResult DownloadDetalhesNotaEntradaConferencia(int id)
        {
            var relatorioRequest = new DetalhesNotaEntradaConferenciaRequest
            {
                IdEmpresa = IdEmpresa,
                NomeUsuario = LabelUsuario,
                IdNotaFiscal = id
            };

            byte[] relatorio = _relatorioService.GerarDetalhesNotaEntradaConferencia(relatorioRequest);

            return File(relatorio, "application/pdf", "Detalhes Nota Fiscal Entrada Conferencia.pdf");
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.List)]
        public JsonResult ImprimirDetalhesEntradaConferencia(BOImprimirDetalhesEntradaConferenciaViewModel viewModel)
        {
            try
            {
                ValidateModel(viewModel);

                var relatorioRequest = new ImprimirDetalhesNotaEntradaConferenciaRequest
                {
                    IdEmpresa = IdEmpresa,
                    NomeUsuario = LabelUsuario,
                    IdNotaFiscal = viewModel.IdNotaFiscal,
                    IdImpressora = viewModel.IdImpressora
                };

                _relatorioService.ImprimirDetalhesNotaEntradaConferencia(relatorioRequest);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Impressão enviada com sucesso."
                }, JsonRequestBehavior.DenyGet);

            }
            catch (Exception e)
            {
                _applicationLogService.Error(ApplicationEnum.BackOffice, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Ocorreu um erro na impressão."
                }, JsonRequestBehavior.DenyGet);
            }
        }




        [ApplicationAuthorize(Permissions = Permissions.Recebimento.RegistrarRecebimento)]
        public JsonResult ValidarNotaRecebimento(string  chaveAcesso, long? idFornecedor, int? numeroNF, string serie, decimal? valor, int? quantidadeVolumes)
        {
            //Valida chave de cesso
            var chaveValida = false;

            if (string.IsNullOrWhiteSpace(chaveAcesso))
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Informe a chave de acesso da NF-e.",
                });
            }

            if (chaveAcesso.Length != 44)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Chave de acesso não possui 44 digitos.",
                });
            }

            string chaveacessosemdigito = chaveAcesso.Substring(0, 43);
            string digitochaveacesso = chaveAcesso.Substring(chaveAcesso.Length - 1);

            Int32 Peso = 2, Soma = 0, Contador, Digito;

            for (Contador = (chaveacessosemdigito.Length - 1); Contador >= 0; Contador--)
            {
                Soma = Soma + (Convert.ToInt32(chaveacessosemdigito[Contador].ToString()) * Peso);

                if (Peso < 9)
                    Peso++;
                else
                    Peso = 2;
            }

            Digito = 11 - (Soma % 11);

            if (Digito > 9) Digito = 0;

            if (digitochaveacesso == Digito.ToString())
                chaveValida = true;
            else
                chaveValida = false;

            if (!chaveValida)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Chave da NF-e invalida.",
                });
            }


            //Verificar se NF já cadastrada no sistema.
            var notafiscal = _uow.NotaFiscalRepository.ObterPorChave(chaveAcesso);
            var notafiscalRecebimento = _uow.NotaFiscalRecebimentoRepository.ObterPorChave(chaveAcesso);

            if (notafiscal != null)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Nota Fiscal já existe e foi efetivada no sistema.",
                });
            }


            if (notafiscalRecebimento != null)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Nota Fiscal já foi registrada no sistema.",
                });
 
            }

            if (!(idFornecedor > 0))
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Informe o Fornecedor."
                });
            }

            if (!(numeroNF > 0))
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Informe o número da Nota Fiscal."
                });
            }

            if (string.IsNullOrWhiteSpace(serie))
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Informe a série da NF-e.",
                });
            }

            if (!(valor > 0))
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Informe o valor da Nota Fiscal para confirmar o recebimento."
                });
            }

            if (!(quantidadeVolumes > 0))
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Informe a quantidade de volumes para confirmar o recebimento."
                });
            }

            return Json(new AjaxGenericResultModel
            {
                Success = true
            });
        }

        [ApplicationAuthorize(Permissions = Permissions.Recebimento.RegistrarRecebimento)]
        public JsonResult ValidarModalRegistroRecebimento(long id)
        {
            var lote = _uow.LoteRepository.PesquisarLotePorNotaFiscal(id);
            var notafiscal = _uow.NotaFiscalRepository.GetById(id);

            if (lote != null || notafiscal.IdNotaFiscalStatus != NotaFiscalStatusEnum.AguardandoRecebimento)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Recebimento da mecadoria já se enconta efetivado no sistema.",
                });
            }

            ImpressaoItem impressaoItem = _uow.ImpressaoItemRepository.Obter(5);

            if (!_uow.BOPrinterRepository.ObterPorPerfil(IdPerfilImpressora, impressaoItem.IdImpressaoItem).Any())
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Não há impressora configurada para Etiqueta de Recebimento.",
                });
            }

            return Json(new AjaxGenericResultModel
            {
                Success = true
            });
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.RegistrarRecebimento)]
        public ActionResult ExibirModalRegistroRecebimento(long id)
        {
            var modal = new BORegistroRecebimentoViewModel
            {
                IdNotaFiscal = id
            };

            return PartialView("RegistroRecebimento", modal);
        }


        [HttpGet]
        [ApplicationAuthorize]
        public ActionResult NotaRecebimento()
        {
            var viewModel = new BONotaRecebimentoViewModel
            {
            };

            return PartialView("NotaRecebimento", viewModel);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.RegistrarRecebimento)]
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

        [ApplicationAuthorize(Permissions = Permissions.Recebimento.RegistrarRecebimento)]
        public ActionResult CarregarDadosNotaFiscalRegistro(string id)
        {
            var notafiscal = _uow.NotaFiscalRepository.GetById(Convert.ToInt64(id));
            var dataAtual = DateTime.Now;

            var model = new BORegistroRecebimentoViewModel
            {
                ChaveAcesso = notafiscal.ChaveAcesso,
                DataRecebimento = dataAtual.ToString("dd/MM/yyyy"),
                HoraRecebimento = dataAtual.ToString("HH:mm:ss"),
                FornecedorNome = string.Concat(notafiscal.Fornecedor.CodigoIntegracao.ToString(), " - ", notafiscal.Fornecedor.NomeFantasia),
                NumeroSerieNotaFiscal = string.Format("{0}-{1}", notafiscal.Numero, notafiscal.Serie),
                ValorTotal = notafiscal.ValorTotal.ToString("n2"),
                DataAtual = dataAtual,
                ValorFrete = notafiscal.ValorFrete.ToString("n2"),
                NumeroConhecimento = notafiscal.NumeroConhecimento,
                TransportadoraNome = string.Concat(notafiscal.Transportadora.CodigoIntegracao.ToString(), " - ", notafiscal.Transportadora.NomeFantasia),
                Peso = notafiscal.PesoBruto.HasValue ? notafiscal.PesoBruto.Value.ToString("n2") : null,
                QtdVolumes = notafiscal.Quantidade == 0 ? (int?)null : notafiscal.Quantidade,
                NotaFiscalPesquisada = true
            };

            return PartialView("RegistroRecebimentoDetalhes", model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.RegistrarRecebimento)]
        public async Task<JsonResult> SalvarNotaRecebimentoDiv(string chaveAcesso, long idFornecedor, int numeroNF, string serie, decimal valor, int quantidadeVolumes)
        {
            try
            {
                await _notaFiscalService.RegistrarRecebimentoNotaFiscalDiv(User.Identity.GetUserId(), 
                                                                           chaveAcesso,
                                                                           idFornecedor,
                                                                           numeroNF,
                                                                           serie,
                                                                           valor,
                                                                           quantidadeVolumes).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _applicationLogService.Error(ApplicationEnum.BackOffice, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Não foi possível salvar a nota fiscal de recebimento. Tente novamente."
                });
            }

            return Json(new AjaxGenericResultModel
            {
                Success = true,
                Message = "Recebimento da nota fiscal registrado com sucesso."
            });
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.RegistrarRecebimento)]
        public async Task<JsonResult> RegistrarRecebimentoNota(long idNotaFiscal, DateTime dataRecebimento, int qtdVolumes, bool notaFiscalPesquisada)
        {
            var lote = _uow.LoteRepository.PesquisarLotePorNotaFiscal(idNotaFiscal);
            var notafiscal = _uow.NotaFiscalRepository.GetById(idNotaFiscal);

            if (lote != null || notafiscal.IdNotaFiscalStatus != NotaFiscalStatusEnum.AguardandoRecebimento)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Recebimento da mecadoria já se enconta efetivado no sistema.",
                });
            }

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
                await _notaFiscalService.RegistrarRecebimentoNotaFiscal(idNotaFiscal, User.Identity.GetUserId(), dataRecebimento, qtdVolumes).ConfigureAwait(false);
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
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.List)]
        public JsonResult ImprimirRelatorioNotas(BOImprimirRelatorioNotasViewModel viewModel)
        {
            try
            {
                ValidateModel(viewModel);

                var request = Mapper.Map<ImprimirRelatorioRecebimentoNotasRequest>(viewModel);

                request.IdEmpresa = IdEmpresa;
                request.NomeUsuario = LabelUsuario;

                _relatorioService.ImprimirRelatorioRecebimentoNotas(request);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Impressão enviada com sucesso."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _applicationLogService.Error(ApplicationEnum.BackOffice, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Ocorreu um erro na impressão."
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.RegistrarRecebimento)]
        public JsonResult ImprimirEtiquetaRecebimento(BOImprimirEtiquetaRecebimentoViewModel viewModel)
        {
            try
            {
                ValidateModel(viewModel);

                Lote lote = _uow.LoteRepository.ObterLoteNota(viewModel.IdNotaFiscal);
                _etiquetaService.ImprimirEtiquetaVolumeRecebimento(lote.IdLote, viewModel.IdImpressora);

                //Registra a impressão da etiqueta de Recebimento
                var logEtiquetagem = new Services.Model.LogEtiquetagem.LogEtiquetagem
                {
                    IdTipoEtiquetagem = TipoEtiquetagemEnum.Recebimento.GetHashCode(),
                    IdEmpresa = IdEmpresa,
                    Quantidade = lote.QuantidadeVolume,
                    IdUsuario = User.Identity.GetUserId()
                };

                _logEtiquetagemService.Registrar(logEtiquetagem);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Impressão realizada com sucesso."
                }, JsonRequestBehavior.DenyGet);

            }
            catch (Exception e)
            {
                _applicationLogService.Error(ApplicationEnum.BackOffice, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Ocorreu um erro na impressão."
                }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.List)]
        public ActionResult DetalhesEntradaConferencia(long id)
        {
            NotaFiscal notaFiscal = _uow.NotaFiscalRepository.GetById(id);

            var model = new BODetalhesEntradaConferenciaViewModel
            {
                IdNotaFiscal = notaFiscal.IdNotaFiscal,
                ChaveAcesso = notaFiscal.ChaveAcesso,
                NumeroNotaFiscal = string.Concat(notaFiscal.Numero.ToString(), " - ", notaFiscal.Serie),
                StatusNotaFiscal = notaFiscal.NotaFiscalStatus.ToString(),
                Fornecedor = string.Concat(notaFiscal.Fornecedor.IdFornecedor.ToString(), " - ", notaFiscal.Fornecedor.NomeFantasia),
                DataCompra = notaFiscal.DataEmissao.ToString("dd/MM/yyyy"),
                PrazoRecebimento = notaFiscal.PrazoEntregaFornecedor.ToString("dd/MM/yyyy"),
                FornecedorCNPJ = StringExtension.CnpjOuCpf(notaFiscal.Fornecedor.CNPJ),
                ValorTotal = notaFiscal.ValorTotal.ToString("C"),
                ValorFrete = notaFiscal.ValorFrete.ToString("C"),
                NumeroConhecimento = notaFiscal.NumeroConhecimento.ToString(),
                PesoConhecimento = notaFiscal.PesoBruto.HasValue ? notaFiscal.PesoBruto.Value.ToString("F") : null,
                TransportadoraNome = string.Concat(notaFiscal.Transportadora.IdTransportadora.ToString(), " - ", notaFiscal.Transportadora.NomeFantasia),
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
                model.StatusNotaFiscal = lote.LoteStatus.Descricao;
                model.Observacao = lote.ObservacaoDivergencia;

                if (lote.DataRecebimento > notaFiscal.PrazoEntregaFornecedor)
                {
                    TimeSpan atraso = lote.DataRecebimento.Subtract(notaFiscal.PrazoEntregaFornecedor);
                    model.DiasAtraso = atraso.Days.ToString();
                }

                //Verifica se o lote está em conferência ou já foi conferido.
                if (lote.IdLoteStatus != LoteStatusEnum.AguardandoRecebimento && lote.IdLoteStatus != LoteStatusEnum.Recebido)
                {
                    model.EmConferenciaOuConferido = true;

                    //Captura as conferências do lote.
                    var loteConferencia = _uow.LoteConferenciaRepository.ObterPorId(lote.IdLote).OrderByDescending(x => x.DataHoraFim).ToList();

                    if (loteConferencia.Count > 0)
                    {
                        model.ConferenciaTipo = loteConferencia.FirstOrDefault().TipoConferencia.Descricao;

                        //Captura o primeiro conferente.
                        model.UsuarioConferencia = _uow.PerfilUsuarioRepository.GetByUserId(loteConferencia.FirstOrDefault().UsuarioConferente.Id).Nome;

                        //Captura a menor data de início da conferência.
                        model.DataInicioConferencia = lote.DataInicioConferencia.HasValue ? lote.DataInicioConferencia.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;

                        model.DataFimConferencia = lote.DataFinalConferencia.HasValue ? lote.DataFinalConferencia.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;

                        List<UsuarioEmpresa> usuarios = _uow.UsuarioEmpresaRepository.ObterPorEmpresa(IdEmpresa);

                        //Calcula o tempo total.
                        foreach (var item in loteConferencia)
                        {
                            var entradaConferenciaItem = new BODetalhesEntradaConferenciaItem
                            {
                                Referencia = item.Produto.Referencia,
                                DescricaoProduto = item.Produto.Descricao,
                                Quantidade = item.Quantidade,
                                DataInicioConferencia = item.DataHoraInicio.ToString("dd/MM/yyyy HH:mm:ss"),
                                DataFimConferencia = item.DataHoraFim.ToString("dd/MM/yyyy HH:mm:ss"),
                                TempoConferencia = item.Tempo.ToString("HH:mm:ss"),
                                UsuarioConferencia = usuarios.Where(x => x.UserId.Equals(item.UsuarioConferente.Id)).FirstOrDefault()?.PerfilUsuario.Nome
                            };

                            model.Items.Add(entradaConferenciaItem);
                        }

                        if (lote.IdLoteStatus != LoteStatusEnum.Conferencia)
                        {
                            model.TempoTotalConferencia = lote.TempoTotalConferencia.HasValue ? TimeSpan.FromSeconds(lote.TempoTotalConferencia.Value).ToString("h'h 'm'm 's's'") : string.Empty;
                        }
                    }

                    model.ConferidoDivergencia = lote.IdLoteStatus == LoteStatusEnum.ConferidoDivergencia;

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
                                NotaFiscal = string.Concat(notaFiscal.Numero.ToString(), " - ", notaFiscal.Serie),
                                IdLote = lote.IdLote,
                                StatusNotasFiscal = lote.LoteStatus.Descricao,
                                UsuarioTratamento = _uow.PerfilUsuarioRepository.GetByUserId(loteDivergencias.First().IdUsuarioDivergencia).Nome,
                                DataTratamento = loteDivergencias.First().DataTratamentoDivergencia.Value.ToString("dd/MM/yyyy hh:mm:ss")
                            };

                            foreach (LoteDivergencia divergencia in loteDivergencias)
                            {
                                List<NotaFiscalItem> nfItem = divergencia.NotaFiscal.NotaFiscalItens.Where(w => w.Produto.IdProduto == divergencia.Produto.IdProduto).ToList();

                                var divergenciaItem = new ExibirDivergenciaRecebimentoItemViewModel
                                {
                                    Referencia = divergencia.Produto.Referencia,
                                    QuantidadeConferencia = divergencia.QuantidadeConferencia,
                                    QuantidadeMais = divergencia.QuantidadeConferenciaMais ?? 0,
                                    QuantidadeMenos = divergencia.QuantidadeConferenciaMenos ?? 0,
                                    QuantidadeNotaFiscal = nfItem == null ? 0 : nfItem.Sum(s => s.Quantidade),
                                    QuantidadeMaisTratado = divergencia.QuantidadeDivergenciaMais ?? 0,
                                    QuantidadeMenosTratado = divergencia.QuantidadeDivergenciaMenos ?? 0,
                                    DescricaoProduto = divergencia.Produto.Descricao,
                                };

                                divergenciaViewModel.Divergencias.Add(divergenciaItem);
                            }

                            model.Divergencias = divergenciaViewModel;
                        }
                    }
                }
            }

            return View(model);
        }

        [ApplicationAuthorize(Permissions = Permissions.Recebimento.ConferirLote)]
        public async Task<JsonResult> ValidarInicioConferencia(long id)
        {
            try
            {
                var empresaConfig = _uow.EmpresaConfigRepository.ConsultarPorIdEmpresa(IdEmpresa);
                var lote = _uow.LoteRepository.PesquisarLotePorNotaFiscal(id);

                //Verifica se o lote já foi conferido durante o processo de conferência.
                if (lote.IdLoteStatus != LoteStatusEnum.Recebido && lote.IdLoteStatus != LoteStatusEnum.Conferencia)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = $"A conferência do lote: {lote.IdLote} já foi finalizada.",
                    });
                }

                if (empresaConfig.TipoConferencia == null)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Nenhum tipo de conferência configurado para a empresa Unidade: " + empresaConfig.Empresa.Sigla + ".",
                    });
                }

                ImpressaoItem impressaoItem = _uow.ImpressaoItemRepository.Obter(2);

                if (!_uow.BOPrinterRepository.ObterPorPerfil(IdPerfilImpressora, impressaoItem.IdImpressaoItem).Any())
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Não há impressora configurada para Etiqueta de Lote.",
                    });
                }

                impressaoItem = _uow.ImpressaoItemRepository.Obter(7);

                if (!_uow.BOPrinterRepository.ObterPorPerfil(IdPerfilImpressora, impressaoItem.IdImpressaoItem).Any())
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Não há impressora configurada para Etiqueta de Devolução.",
                    });
                }

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
            catch (Exception e)
            {
                _applicationLogService.Error(ApplicationEnum.BackOffice, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Algo inesperado ocorreu, atualize a página e tente novamente."
                });
            }
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.ConferirLote)]
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
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "As configurações da empresa não foram encontradas. Por favor, tente novamente!");

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

            if (!lote.DataInicioConferencia.HasValue)
            {
                lote.DataInicioConferencia = DateTime.Now;
                _uow.SaveChanges();
            }

            ////Se o tipo da conferência for, o usuário não poderá informar a quantidade por caixa e quantidade de caixa.
            ////Sabendo disso, atribui 1 para os campos.
            //if (empresaConfig.TipoConferencia.IdTipoConferencia == TipoConferenciaEnum.ConferenciaCemPorcento)
            //{
            //    model.QuantidadePorCaixa = 1;
            //    model.QuantidadeCaixa = 1;
            //}

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.ConferirLote)]
        public async Task<ActionResult> ObterDadosReferenciaConferencia(string codigoBarrasOuReferencia, long idLote)
        {
            //Validações do produto.
            var conferencia = _conferenciaService.ValidarProduto(idLote, codigoBarrasOuReferencia, IdEmpresa);

            if (!conferencia.Sucesso)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = conferencia.Sucesso,
                    Message = conferencia.Mensagem
                });
            }

            //Captura as quantidade conferida e não conferida do lote.
            int quantidadeConferida = 0;
            int quantidadeNaoConferida = 0;

            _conferenciaService.ConsultarQuantidadeConferidaENaoConferida(conferencia.Lote, conferencia.Produto, ref quantidadeConferida, ref quantidadeNaoConferida);

            //Captura o Usuário que está iniciando a conferência novamente.
            var usuario = _uow.PerfilUsuarioRepository.GetByUserId(User.Identity.GetUserId());

            var model = new BOEntradaConferenciaViewModel
            {
                IdNotaFiscal = conferencia.Lote.NotaFiscal.IdNotaFiscal,
                IdLote = conferencia.Lote.IdLote,
                NumeroNotaFiscal = string.Concat(conferencia.Lote.NotaFiscal.Numero, " - ", conferencia.Lote.NotaFiscal.Serie),
                IdUuarioConferente = usuario.UsuarioId,
                NomeConferente = usuario.Nome,
                DataHoraRecebimento = conferencia.Lote.DataRecebimento.ToString("dd/MM/yyyy HH:mm"),
                NomeFornecedor = conferencia.Lote.NotaFiscal.Fornecedor.NomeFantasia,
                QuantidadeVolume = conferencia.Lote.QuantidadeVolume,
                TipoConferencia = conferencia.EmpresaConfig.TipoConferencia.Descricao,
                IdTipoConferencia = conferencia.EmpresaConfig.TipoConferencia.IdTipoConferencia.GetHashCode(),
                Referencia = conferencia.Produto.Referencia,
                DescricaoReferencia = conferencia.Produto.Descricao,
                Embalagem = conferencia.Produto.MultiploVenda.ToString("N2"),
                Unidade = conferencia.Produto.UnidadeMedida.Sigla,
                QuantidadeEstoque = conferencia.ProdutoEstoque == null ? 0 : conferencia.ProdutoEstoque.Saldo,
                QuantidadeNaoConferida = quantidadeNaoConferida,
                QuantidadeConferida = quantidadeConferida,
                InicioConferencia = DateTime.Now.ToString(),
                QuantidadePorCaixa = null,
                Multiplo = conferencia.Produto.MultiploVenda,
                QuantidadeReservada = await _produtoService.ConsultarQuantidadeReservada(conferencia.Produto.IdProduto, IdEmpresa),
                MediaVenda = conferencia.ProdutoEstoque.MediaVenda.HasValue ? conferencia.ProdutoEstoque.MediaVenda.Value.ToString("N2") : string.Empty,
                QuantidadeCaixa = conferencia.EmpresaConfig.IdTipoConferencia == TipoConferenciaEnum.ConferenciaCemPorcento ? 1 : null as int?
            };

            if (conferencia.ProdutoEstoque == null || (conferencia.ProdutoEstoque != null && conferencia.ProdutoEstoque.EnderecoArmazenagem == null))
            {
                model.Localizacao = string.Empty;
                model.EnviarPicking = true;
            }
            else
            {
                model.Localizacao = conferencia.ProdutoEstoque.EnderecoArmazenagem.Codigo;
                model.EnviarPicking = conferencia.ProdutoEstoque.Saldo == 0 ? true : false;
            }

            string json = JsonConvert.SerializeObject(model);

            return Json(new AjaxGenericResultModel
            {
                Success = true,
                Message = conferencia.Mensagem,
                Data = json
            });
        }

        [HttpPost]
        public JsonResult VerificarDiferencaMultiploConferencia(string codigoBarrasOuReferencia, int quantidadePorCaixa, decimal multiplo, long idLote)
        {
            var lote = _uow.LoteRepository.GetById(idLote);

            //Verifica se o lote já foi conferido durante o processo de conferência.
            if (lote.IdLoteStatus != LoteStatusEnum.Recebido && lote.IdLoteStatus != LoteStatusEnum.Conferencia)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = $"A conferência do lote: {lote.IdLote} já foi finalizada.",
                });
            }

            //Valida novamente se a referência é valida.
            if (string.IsNullOrEmpty(codigoBarrasOuReferencia))
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Referência inválida. Por favor, tente novamente!"
                });
            }

            var referencia = _uow.ProdutoRepository.ConsultarPorCodigoBarrasOuReferencia(codigoBarrasOuReferencia);

            //Valida se a referencia (peça) foi encontrado.
            if (referencia == null)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Referência sem código de barras. Por favor, tente novamente!"
                });
            }

            //Valida se o múltilo é igual ou menor 0.
            if (multiplo < 0 || multiplo == 0)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Múltiplo inválido. Por favor, tente novamente!"
                });
            }

            //Verifica se o múltiplo é igual.
            if (referencia.MultiploVenda == multiplo)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = string.Empty
                });
            }

            if (quantidadePorCaixa % multiplo != 0)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Quantidade informada não confere com o múltiplo. Para prosseguir, é necessário a validação do Coordenador."
                });
            }
            else
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "O múltiplo está diferente ao do cadastro. Para prosseguir, é necessário a validação do Coordenador."
                });
            }
        }

        [HttpPost]
        public async Task<JsonResult> ValidarAcessoCoordenadorConferencia(string usuario, string senha)
        {
            ApplicationUser applicationUser = await UserManager.FindByNameAsync(usuario);

            //Validar Login
            var retornoLogin = SignInManager.UserManager.CheckPassword(applicationUser, senha);

            if (!retornoLogin)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Usuário ou senha inválidos. Por favor, tente novamente!"
                });
            }

            //validar Permissão
            var permissao = UserManager.GetPermissions(applicationUser.Id).Contains(Permissions.Recebimento.PermitirDiferencaMultiploConferencia);

            if (!permissao)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "O usuário informado não possui permissão para confirmar a diferença de múltiplo. Solicite a permissão para o Administrador."
                });
            }

            return Json(new AjaxGenericResultModel
            {
                Success = true,
                Message = string.Empty
            });
        }

        [HttpPost]
        public async Task<JsonResult> ValidarAcessoMudancaConferenciaManual(string usuario, string senha)
        {
            ApplicationUser applicationUser = await UserManager.FindByNameAsync(usuario);

            //Validar Login
            var retornoLogin = SignInManager.UserManager.CheckPassword(applicationUser, senha);

            if (!retornoLogin)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Usuário ou senha inválidos. Por favor, tente novamente!"
                });
            }

            //validar Permissão
            var permissao = UserManager.GetPermissions(applicationUser.Id).Contains(Permissions.Recebimento.PermitirConferenciaManual);

            if (!permissao)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "O usuário informado não possui permissão para permitir a conferência manual. Contate o Administrador."
                });
            }

            return Json(new AjaxGenericResultModel
            {
                Success = true,
                Message = string.Empty
            });
        }

        [ApplicationAuthorize(Permissions = Permissions.Recebimento.ConferirLote)]
        public async Task<JsonResult> RegistrarConferencia(string codigoBarrasOuReferencia, long idLote, int quantidadePorCaixa, int quantidadeCaixa, string inicioConferencia, decimal multiplo, int idTipoConferencia)
        {
            try
            {
                //Validar conferência.
                var conferencia = _conferenciaService.ValidarConferencia(idLote, codigoBarrasOuReferencia, IdEmpresa, idTipoConferencia, quantidadePorCaixa, quantidadeCaixa, multiplo);

                if (!conferencia.Sucesso)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = conferencia.Mensagem
                    });
                }

                //Registrar conferência.
                var conferenciaRegistro = await _conferenciaService.RegistrarConferencia(conferencia.Lote, conferencia.Produto, IdUsuario, inicioConferencia, idTipoConferencia, quantidadePorCaixa, quantidadeCaixa);

                #region Impressão Automática de Etiquetas

                var request = new ImprimirEtiquetaArmazenagemVolume
                {
                    NroLote = idLote,
                    QuantidadeEtiquetas = quantidadeCaixa,
                    QuantidadePorCaixa = quantidadePorCaixa,
                    ReferenciaProduto = conferenciaRegistro.Produto.Referencia,
                    Usuario = _uow.PerfilUsuarioRepository.GetByUserId(User.Identity.GetUserId())?.Nome,
                    IdImpressora = _uow.BOPrinterRepository.ObterPorPerfil(IdPerfilImpressora, _uow.ImpressaoItemRepository.Obter(2).IdImpressaoItem).First().Id
                };

                _etiquetaService.ImprimirEtiquetaArmazenagemVolume(request);

                if (VerificarPecaHaMais(conferencia.Lote.IdLote, quantidadePorCaixa, conferencia.Lote.IdNotaFiscal, conferencia.Produto.IdProduto) > 0)
                {
                    var requestPecasMais = new ImprimirEtiquetaDevolucaoRequest
                    {
                        Linha1 = conferencia.Lote.IdLote.ToString().PadLeft(10, '0'),
                        Linha2 = conferencia.Produto.Referencia,
                        Linha3 = "PC.A+",
                        IdImpressora = _uow.BOPrinterRepository.ObterPorPerfil(IdPerfilImpressora, _uow.ImpressaoItemRepository.Obter(7).IdImpressaoItem).First().Id
                    };

                    _etiquetaService.ImprimirEtiquetaDevolucao(requestPecasMais);

                    //Registra a impressão da etiqueta de Devolução
                    var logEtiquetagemDevolucao = new Services.Model.LogEtiquetagem.LogEtiquetagem
                    {
                        IdTipoEtiquetagem = TipoEtiquetagemEnum.Devolucao.GetHashCode(),
                        IdEmpresa = IdEmpresa,
                        IdProduto = conferenciaRegistro.Produto.IdProduto,
                        Quantidade = quantidadeCaixa,
                        IdUsuario = User.Identity.GetUserId()
                    };

                    _logEtiquetagemService.Registrar(logEtiquetagemDevolucao);
                }

                //Registra a impressão da etiqueta de Lote
                var logEtiquetagem = new Services.Model.LogEtiquetagem.LogEtiquetagem
                {
                    IdTipoEtiquetagem = TipoEtiquetagemEnum.Lote.GetHashCode(),
                    IdEmpresa = IdEmpresa,
                    IdProduto = conferenciaRegistro.Produto.IdProduto,
                    Quantidade = quantidadeCaixa,
                    IdUsuario = User.Identity.GetUserId()
                };

                _logEtiquetagemService.Registrar(logEtiquetagem);

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

        [HttpPost]
        public JsonResult ObterTipoConferencia()
        {
            var empresaConfig = _uow.EmpresaConfigRepository.ConsultarPorIdEmpresa(IdEmpresa);

            if (empresaConfig == null)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = string.Empty,
                    Data = "0"
                });
            }

            return Json(new AjaxGenericResultModel
            {
                Success = true,
                Message = empresaConfig.TipoConferencia.Descricao,
                Data = empresaConfig.TipoConferencia.IdTipoConferencia.GetHashCode().ToString()
            });
        }

        [HttpPost]
        public JsonResult ConsultarPecasHaMaisConferencia(string codigoBarrasOuReferencia, long idLote, int quantidadePorCaixa)
        {
            int pecasHaMais = 0;

            try
            {
                if (quantidadePorCaixa == 0)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "O campo quantidade por caixa não pode ser 0. Por favor, tente novamente!"
                    });
                }

                //Captura o id da nota fiscal com base no id do lote.
                var idNotaFiscal = _uow.LoteRepository.GetById(idLote).IdNotaFiscal;

                if (idNotaFiscal != 0)
                {
                    //Captura o id do produto.
                    var idProduto = _uow.ProdutoRepository.ConsultarPorCodigoBarrasOuReferencia(codigoBarrasOuReferencia).IdProduto;

                    if (idProduto != 0)
                    {
                        pecasHaMais = VerificarPecaHaMais(idLote, quantidadePorCaixa, idNotaFiscal, idProduto);
                    }
                }
            }
            catch (Exception)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Data = Convert.ToString(pecasHaMais)
                });
            }

            return Json(new AjaxGenericResultModel
            {
                Success = true,
                Data = Convert.ToString(pecasHaMais)
            });
        }

        private int VerificarPecaHaMais(long idLote, int quantidadePorCaixa, long idNotaFiscal, long idProduto)
        {
            int pecasHaMais;
            //Captura os itens da nota fiscal do produto.
            var notaFiscalItem = _uow.NotaFiscalItemRepository.ObterPorItem(idNotaFiscal, idProduto);
            var conferencia = _uow.LoteConferenciaRepository.ObterPorProduto(idLote, idProduto);

            if (notaFiscalItem.Any())
            {
                int qtdConferida = conferencia.Sum(s => s.Quantidade);
                int quantidadePecasNota = notaFiscalItem.Sum(s => s.Quantidade);

                pecasHaMais = (qtdConferida + quantidadePorCaixa) - quantidadePecasNota;
                pecasHaMais = pecasHaMais < 0 ? 0 : pecasHaMais;

                if (pecasHaMais > quantidadePorCaixa)
                {
                    pecasHaMais = quantidadePorCaixa;
                }
            }
            else
            {
                pecasHaMais = quantidadePorCaixa;
            }

            return pecasHaMais;
        }

        [ApplicationAuthorize(Permissions = Permissions.Recebimento.RegistrarRecebimento)]
        public JsonResult ValidarModalTratarDivergencia(long id)
        {
            var lote = _uow.LoteRepository.PesquisarLotePorNotaFiscal(id);

            if (lote.IdLoteStatus != LoteStatusEnum.ConferidoDivergencia)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = $"As divergências do lote: {lote.IdLote} já foram tratadas.",
                });
            }

            return Json(new AjaxGenericResultModel
            {
                Success = true
            });
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
                NotaFiscal = string.Concat(notaFiscal.Numero, " - ", notaFiscal.Serie),
                IdNotaFiscal = notaFiscal.IdNotaFiscal,
                StatusNotasFiscal = notaFiscal.NotaFiscalStatus.Descricao
            };

            List<LoteDivergencia> loteDivergencias = _uow.LoteDivergenciaRepository.RetornarPorNotaFiscal(id);

            if (loteDivergencias.Any())
            {
                divergenciaViewModel.InicioConferencia = loteConferencia.First().DataHoraInicio.ToString("dd/MM/yyyy hh:mm:ss");
                divergenciaViewModel.FimConferencia = loteConferencia.Last().DataHoraFim.ToString("dd/MM/yyyy hh:mm:ss");
            }

            foreach (LoteDivergencia divergencia in loteDivergencias)
            {
                List<NotaFiscalItem> nfItem = divergencia.NotaFiscal.NotaFiscalItens.Where(w => w.Produto.IdProduto == divergencia.Produto.IdProduto).ToList();

                var divergenciaItem = new TratarDivergenciaRecebimentoItemViewModel
                {
                    IdLoteDivergencia = divergencia.IdLoteDivergencia,
                    Referencia = divergencia.Produto.Referencia,
                    QuantidadeConferencia = divergencia.QuantidadeConferencia,
                    QuantidadeMais = divergencia.QuantidadeConferenciaMais ?? 0,
                    QuantidadeMenos = divergencia.QuantidadeConferenciaMenos ?? 0,
                    QuantidadeNotaFiscal = nfItem == null ? 0 : nfItem.Sum(s => s.Quantidade),
                    DescricaoProduto = divergencia.Produto.Descricao
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
                var lote = _uow.LoteRepository.PesquisarLotePorNotaFiscal(viewModel.IdNotaFiscal);

                if (lote.IdLoteStatus != LoteStatusEnum.ConferidoDivergencia)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = $"As divergências do lote: {lote.IdLote} já foram tratadas.",
                    });
                }

                if (!ModelState.IsValid)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Não foi possível tratar as divergências, formulário está inválido."
                    }, JsonRequestBehavior.AllowGet);
                }

                foreach (var divergencia in viewModel.Divergencias)
                {
                    if (divergencia.QuantidadeMaisTratado.HasValue == false && divergencia.QuantidadeMenosTratado.HasValue == false)
                    {
                        return Json(new AjaxGenericResultModel
                        {
                            Success = false,
                            Message = "Existem divergências não tratadas."
                        }, JsonRequestBehavior.DenyGet);
                    }
                }

                var request = Mapper.Map<TratarDivergenciaRequest>(viewModel);
                request.IdUsuario = User.Identity.GetUserId();
                request.IdEmpresa = IdEmpresa;

                LoteStatusEnum statusLote = await _loteService.TratarDivergencia(request, IdUsuario).ConfigureAwait(false);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = statusLote == LoteStatusEnum.AguardandoCriacaoNFDevolucao ? "Iniciando a criação da nota fiscal de devolução." : "Tratamento de divergências finalizado."
                }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception e)
            {
                _applicationLogService.Error(ApplicationEnum.BackOffice, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = e is BusinessException ? e.Message : "Não foi possível finalizar tratamento de divergências."
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
                NotaFiscal = string.Concat(notaFiscal.Numero.ToString(), " - ", notaFiscal.Serie),
                IdLote = lote.IdLote,
                StatusNotasFiscal = lote.LoteStatus.Descricao,
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
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.RelatorioResumoEtiquetagem)]
        public ActionResult RelatorioResumoEtiquetagem()
        {
            var model = new RelatorioResumoEtiquetagemViewModel
            {
                Filter = new RelatorioResumoEtiquetagemFilterViewModel()
                {
                    ListaTipoEtiquetagem = new SelectList(
                    _uow.TipoEtiquetagemRepository.Todos().Select(x => new SelectListItem
                    {
                        Value = x.IdTipoEtiquetagem.GetHashCode().ToString(),
                        Text = x.Descricao
                    }), "Value", "Text")
                }
            };

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.RelatorioResumoEtiquetagem)]
        public ActionResult RelatorioResumoEtiquetagemPageData(DataTableFilter<RelatorioResumoEtiquetagemFilterViewModel> model)
        {
            var filtros = Mapper.Map<DataTableFilter<LogEtiquetagemListaFiltro>>(model);

            IEnumerable<LogEtiquetagemListaLinhaTabela> result = _uow.LogEtiquetagemRepository.BuscarLista(filtros, out int registrosFiltrados, out int totalRegistros, IdEmpresa);

            var relatorioResumoEtiquetagemListItemViewModel = new List<RelatorioResumoEtiquetagemListItemViewModel>();

            List<UsuarioEmpresa> usuarios = _uow.UsuarioEmpresaRepository.ObterPorEmpresa(IdEmpresa);

            foreach (var item in result)
            {
                relatorioResumoEtiquetagemListItemViewModel.Add(new RelatorioResumoEtiquetagemListItemViewModel()
                {
                    IdLogEtiquetagem = item.IdLogEtiquetagem,
                    Referencia = item.Referencia,
                    Descricao = item.Descricao,
                    TipoEtiquetagem = item.TipoEtiquetagem,
                    Quantidade = item.Quantidade,
                    DataHora = item.DataHora.ToString("dd/MM/yyyy HH:mm:ss"),
                    Usuario = usuarios.Where(x => x.UserId.Equals(item.Usuario)).FirstOrDefault()?.PerfilUsuario.Nome
                });
            }

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = relatorioResumoEtiquetagemListItemViewModel
            });
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.RelatorioResumoEtiquetagem)]
        public ActionResult DownloadRelatorioResumoEtiquetagem(RelatorioResumoEtiquetagemRequest viewModel)
        {
            ValidateModel(viewModel);

            viewModel.IdEmpresa = IdEmpresa;
            viewModel.NomeUsuario = User.Identity.Name;

            byte[] relatorio = _relatorioService.GerarRelatorioResumoEtiquetagem(viewModel);

            return File(relatorio, "application/pdf", "Relatório Resumo Etiquetagem.pdf");
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.RelatorioRastreioPeca)]
        public ActionResult RelatorioRastreioPeca()
        {
            return View(new RelatorioRastreioPecaViewModel());
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.RelatorioRastreioPeca)]
        public ActionResult RelatorioRastreioPecaPageData(DataTableFilter<RelatorioRastreioPecaFilterViewModel> model)
        {
            model.CustomFilter.IdEmpresa = IdEmpresa;

            var list = _uow.LoteConferenciaRepository.RastreioPeca(model.CustomFilter, out int registrosFiltrados, out int totalRegistros);

            var result = list.PaginationResult(model).Select(x => new RelatorioRastreioPecaListItemViewModel
            {
                DataRecebimento = x.DataRecebimento.ToString("dd/MM/yyyy"),
                Empresa = x.Empresa,
                IdEmpresa = x.IdEmpresa,
                IdLote = x.IdLote,
                NroNota = x.NroNota,
                QtdCompra = x.QtdCompra,
                QtdRecebida = x.QtdRecebida,
                ReferenciaProduto = x.ReferenciaProduto,
                DescricaoProduto = x.DescricaoProduto
            });

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = result
            });
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.RelatorioRastreioPeca)]
        public ActionResult DownloadRelatorioRastreioPeca(RelatorioRastreioPecaRequest viewModel)
        {
            ValidateModel(viewModel);

            viewModel.IdEmpresa = IdEmpresa;

            byte[] relatorio = _relatorioService.GerarRelatorioRastreioPeca(viewModel, User.Identity.GetUserId());

            return File(relatorio, "application/pdf", "Relatório Rastreio de Peças.pdf");
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.ConferirLote)]
        public async Task<JsonResult> FinalizarConferencia(long id)
        {
            try
            {
                var lote = _uow.LoteRepository.GetById(id);

                //Verifica se o lote já foi conferido durante o processo de conferência.
                if (lote.IdLoteStatus != LoteStatusEnum.Recebido && lote.IdLoteStatus != LoteStatusEnum.Conferencia)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = $"A conferência do lote: {lote.IdLote} já foi finalizada.",
                    });
                }

                await _loteService.FinalizarConferencia(id, User.Identity.GetUserId()).ConfigureAwait(false);
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
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.ListarResumoProducao)]
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
                DateMax = model.CustomFilter.DataRecebimentoMaxima.HasValue ?
                    new DateTime(model.CustomFilter.DataRecebimentoMaxima.Value.Year,
                                 model.CustomFilter.DataRecebimentoMaxima.Value.Month,
                                 model.CustomFilter.DataRecebimentoMaxima.Value.Day, 23, 59, 59)
                    : model.CustomFilter.DataRecebimentoMaxima,
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
                Percentual = x.PERCENTUAL.ToString("N2"),
                Ranking = x.RANKING
            }).PaginationResult(model);

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
                DateMax = model.CustomFilter.DataRecebimentoMaxima.HasValue ?
                    new DateTime(model.CustomFilter.DataRecebimentoMaxima.Value.Year,
                                 model.CustomFilter.DataRecebimentoMaxima.Value.Month,
                                 model.CustomFilter.DataRecebimentoMaxima.Value.Day, 23, 59, 59)
                    : model.CustomFilter.DataRecebimentoMaxima,
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
            }).PaginationResult(model);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = total,
                RecordsFiltered = list.Count(),
                Data = list
            });
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.TratarDivergencia)]
        public async Task<JsonResult> ContinuarProcessamentoLote(long id)
        {
            try
            {
                var lote = _uow.LoteRepository.ObterLoteNota(id);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Data = (lote.IdLoteStatus == LoteStatusEnum.AguardandoCriacaoNFDevolucao).ToString(),
                }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception e)
            {
                _applicationLogService.Error(ApplicationEnum.BackOffice, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Data = "false",
                    Message = e is BusinessException ? e.Message : "Não foi possivel verificar o status do lote."
                }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.TratarDivergencia)]
        public async Task<JsonResult> FinalizarProcessamentoTratativaDivergencia(long id)
        {
            try
            {
                ProcessamentoTratativaDivergencia processamento = await _loteService.FinalizarProcessamentoTratativaDivergencia(id, IdUsuario).ConfigureAwait(false);
                string json = JsonConvert.SerializeObject(processamento);

                return Json(new AjaxGenericResultModel
                {
                    Success = !processamento.ProcessamentoErro,
                    Message = processamento.ProcessamentoErro ? "Não foi possivel finalizar a tratativa de divergência." : "Trativa de Divergência finalizada com sucesso.",
                    Data = json
                }, JsonRequestBehavior.DenyGet); ;
            }
            catch (Exception e)
            {
                _applicationLogService.Error(ApplicationEnum.BackOffice, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Data = "false",
                    Message = e is BusinessException ? e.Message : "Não foi possivel finalizar a tratativa de divergência."
                }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Recebimento.TratarDivergencia)]
        public ActionResult ResumoProcessamentoDivergencia(long id)
        {
            NotaFiscal notaFiscal = _uow.NotaFiscalRepository.GetById(id);
            Lote lote = _uow.LoteRepository.ObterLoteNota(id);
            List<LoteConferencia> loteConferencia = _uow.LoteConferenciaRepository.Obter(lote.IdLote);
            List<LoteDivergencia> loteDivergencias = _uow.LoteDivergenciaRepository.RetornarPorNotaFiscal(id);
            PerfilUsuario perfilUsuario = _uow.PerfilUsuarioRepository.GetByUserId(loteConferencia.First().IdUsuarioConferente);

            var divergenciaViewModel = new ExibirDivergenciaRecebimentoViewModel
            {
                ConferidoPor = perfilUsuario.Nome,
                InicioConferencia = lote.DataInicioConferencia.Value.ToString("dd/MM/yyyy hh:mm:ss"),
                FimConferencia = lote.DataFinalConferencia.Value.ToString("dd/MM/yyyy hh:mm:ss"),
                NotaFiscal = string.Concat(notaFiscal.Numero.ToString(), " - ", notaFiscal.Serie),
                IdLote = lote.IdLote,
                StatusNotasFiscal = lote.LoteStatus.Descricao,
                UsuarioTratamento = _uow.PerfilUsuarioRepository.GetByUserId(loteDivergencias.First().IdUsuarioDivergencia).Nome,
                DataTratamento = loteDivergencias.First().DataTratamentoDivergencia.Value.ToString("dd/MM/yyyy hh:mm:ss"),
                Processamento = new ProcessamentoTratativaDivergenciaViewModel()
                {
                    AtualizacaoNFCompra = true,
                    ConfirmacaoNFCompra = true,
                    AtualizacaoEstoque = true,
                    CriacaoNFDevolucao = notaFiscal.CodigoIntegracaoNFDevolucao.HasValue,
                    ConfirmacaoNFDevolucao = notaFiscal.CodigoIntegracaoNFDevolucao.HasValue && notaFiscal.NFDevolucaoConfirmada,
                    AutorizacaoNFDevolucaoSefaz = false,
                    CriacaoQuarentena = _uow.QuarentenaRepository.ExisteQuarentena(lote.IdLote)
                }
            };

            return View(divergenciaViewModel);
        }

        public ActionResult PesquisaLote()
        {
            return View(new PesquisaLoteModalViewModel());
        }

        public ActionResult PesquisaLoteModalPageData(DataTableFilter<PesquisaLoteModalFilterViewModel> model)
        {
            List<PesquisaLoteModalItemViewModel> list = new List<PesquisaLoteModalItemViewModel>();

            var query = _uow.LoteRepository.Todos();

            int totalRecords = query.Count();

            if (model.CustomFilter.NroLote.HasValue)
            {
                query = query.Where(x => x.IdLote == model.CustomFilter.NroLote);
            }

            if (!string.IsNullOrWhiteSpace(model.CustomFilter.CNPJFornecedor))
            {
                string cnpj = Regex.Replace(model.CustomFilter.CNPJFornecedor, @"[^\d]", string.Empty);

                query = query.Where(x => x.NotaFiscal.Fornecedor.CNPJ == cnpj);
            }

            if (model.CustomFilter.NroNota.HasValue)
            {
                query = query.Where(x => x.NotaFiscal.Numero == model.CustomFilter.NroNota);
            }

            if (model.CustomFilter.Recebimento.HasValue)
            {
                var dataInicio = model.CustomFilter.Recebimento.Value.Date;
                var dataFim = model.CustomFilter.Recebimento.Value.Date.AddDays(1);

                query = query.Where(x => x.DataRecebimento >= dataInicio && x.DataRecebimento < dataFim);
            }

            if (model.CustomFilter.CodFornecesor.HasValue)
            {
                query = query.Where(x => x.NotaFiscal.IdFornecedor == model.CustomFilter.CodFornecesor.Value);
            }

            foreach (var item in query.ToList())
            {
                list.Add(new PesquisaLoteModalItemViewModel()
                {
                    NomeFantasiaFormecedor = item.NotaFiscal.Fornecedor.NomeFantasia,
                    NroLote = item.IdLote,
                    NroNota = item.NotaFiscal.Numero,
                    Recebimento = item.DataRecebimento.ToString("dd/MM/yyyy")
                });
            }

            int totalRecordsFiltered = list.Count;

            var result = list.PaginationResult(model);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = totalRecordsFiltered,
                Data = result
            });
        }

    }
}
using AutoMapper;
using DartDigital.Library.Exceptions;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Model.Expedicao;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.ExpedicaoCtx;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class ExpedicaoController : BOBaseController
    {
        private readonly ExpedicaoService _expedicaoService;
        private readonly RelatorioService _relatorioService;
        private readonly EtiquetaService _etiquetaService;
        private readonly ILog _log;

        private readonly UnitOfWork _uow;

        public ExpedicaoController(ExpedicaoService expedicaoService, RelatorioService relatorioService, EtiquetaService etiquetaService, UnitOfWork uow, ILog log)
        {
            _expedicaoService = expedicaoService;
            _relatorioService = relatorioService;
            _etiquetaService = etiquetaService;
            _uow = uow;
            _log = log;
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosExpedicao.RelatorioVolumesInstaladosTransportadora)]
        public ActionResult RelatorioVolumesInstaladosTransportadora()
        {
            return View();
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosExpedicao.RelatorioVolumesInstaladosTransportadora)]
        public ActionResult RelatorioVolumesInstaladosTransportadoraPageData(DataTableFilter<RelatorioVolumesInstaladosTransportadoraFilterViewModel> model)
        {
            var listaRetorno = new List<RelatorioVolumesInstaladosTransportadoraListItemViewModel>();

            var filtro = Mapper.Map<DataTableFilter<RelatorioVolumesInstaladosTransportadoraFiltro>>(model);

            filtro.CustomFilter.IdEmpresa = IdEmpresa;

            var volumesTransportadora = _expedicaoService.BuscarDadosVolumePorTransportadora(filtro, out int totalRecordsFiltered, out int totalRecords);

            volumesTransportadora.ForEach(volume => listaRetorno.Add(new RelatorioVolumesInstaladosTransportadoraListItemViewModel
            {
                Transportadora = $"{volume.IdTransportadora.ToString().PadLeft(3, '0')} - {volume.TransportadoraNome}",
                CodigoEndereco = volume.CodigoEndereco ?? "Não instalado",
                NumeroPedido = volume.NumeroPedido.ToString(),
                NumeroVolume = volume.NumeroVolume.ToString(),
                StatusVolume = volume.StatusVolume
            }));

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = totalRecordsFiltered,
                Data = listaRetorno
            });
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosExpedicao.RelatorioVolumesInstaladosTransportadora)]
        public ActionResult DownloadRelatorioVolumesInstaladosTransportadora(RelatorioVolumesInstaladosTransportadoraFilterViewModel filtro)
        {
            var requisicaoRelatorio = Mapper.Map<RelatorioVolumesInstaladosTransportadoraFiltro>(filtro);

            requisicaoRelatorio.IdEmpresa = IdEmpresa;

            var relatorio = _relatorioService.GerarRelatorioVolumesInstaladosTransportadora(requisicaoRelatorio, LabelUsuario);

            return File(relatorio, "application/pdf", "Relatório - Volumes Instalados X Transportadora.pdf");
        }

        [HttpGet]
        public ActionResult PedidoVendaPesquisaModal()
        {
            return View();
        }

        [HttpPost]
        [ApplicationAuthorize]
        public ActionResult PedidoVendaPesquisaModalDadosLista(DataTableFilter<PedidoVendaModalViewModelFilterViewModel> model)
        {
            var filter = Mapper.Map<DataTableFilter<PedidoVendaFiltro>>(model);

            filter.CustomFilter.IdEmpresa = IdEmpresa;

            var pedidosVendaTabela = _expedicaoService.BuscarDadosPedidoVendaParaTabela(filter, out int registrosFiltrados, out int totalRegistros);

            var listaPedidosVenda = new List<PedidoVendaModalViewModelListItemViewModel>();

            pedidosVendaTabela.ForEach(pv => listaPedidosVenda.Add(new PedidoVendaModalViewModelListItemViewModel
            {
                IdPedidoVenda = pv.IdPedidoVenda,
                NumeroPedido = pv.NumeroPedido,
                NumeroPedidoVenda = pv.NumeroPedidoVenda,
                ClienteNome = pv.ClienteNome,
                TransportadoraNome = pv.TransportadoraNome
            }));

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = listaPedidosVenda
            });
        }

        [ApplicationAuthorize(Permissions = Permissions.RelatoriosExpedicao.MovimentacaoVolumes)]
        public async Task<ActionResult> MovimentacaoVolumes(string dataInicial, string dataFinal, string tipoPagamento, bool? requisicao)
        {
            if (dataInicial.NullOrEmpty() || dataFinal.NullOrEmpty())
            {
                if (!dataInicial.NullOrEmpty())
                {
                    dataFinal = dataInicial;
                }
                else if (!dataFinal.NullOrEmpty())
                {
                    dataInicial = dataFinal;
                }
                else
                {
                    var today = DateTime.Today;

                    dataInicial = today.ToString("dd/MM/yyyy");
                    dataFinal = today.ToString("dd/MM/yyyy");
                }

                var urlRedirecionamento = $"MovimentacaoVolumes?dataInicial={dataInicial}&dataFinal={dataFinal}";

                return Redirect(urlRedirecionamento);
            }

            var dataInicialPtBr = DateTime.Parse(dataInicial, new CultureInfo("pt-BR"), DateTimeStyles.None);
            var dataFinalPtBr = DateTime.Parse(dataFinal, new CultureInfo("pt-BR"), DateTimeStyles.None);

            var viewModel = new MovimentacaoVolumesViewModel();

            viewModel.Filter = new MovimentacaoVolumesFilterViewModel()
            {
                DataInicial = dataInicialPtBr,
                DataFinal = dataFinalPtBr,
                TipoPagamento = tipoPagamento,
                Requisicao = requisicao
            };

            viewModel.ListaTiposPagamento = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Cartão Débito", Value = "CD" },
                new SelectListItem { Text = "Cartão Crédito", Value = "CC" },
                new SelectListItem { Text = "Dinheiro", Value = "D"},
            }, "Value", "Text");

            viewModel.ListaRequisicao = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Não", Value = "false"},
                new SelectListItem { Text = "Sim", Value = "true"},
            }, "Value", "Text");

            var dadosMovimentacaoVolumesIntegracoes = await _expedicaoService.BuscarDadosMovimentacaoVolumesIntegracoes(IdEmpresa).ConfigureAwait(false);

            viewModel.AguardandoIntegracao = dadosMovimentacaoVolumesIntegracoes.AguardandoIntegracao;

            viewModel.AguardandoRobo = dadosMovimentacaoVolumesIntegracoes.AguardandoRobo;

            bool? dinheiro = null, cartaoCredito = null, cartaoDebito = null;

            if (!tipoPagamento.NullOrEmpty())
            {
                if (tipoPagamento == "CC")
                {
                    cartaoCredito = true;
                }

                if (tipoPagamento == "CD")
                {
                    cartaoDebito = true;
                }

                if (tipoPagamento == "D")
                {
                    dinheiro = true;
                }
            }

            var dadosRetorno = _expedicaoService.BuscarDadosMovimentacaoVolumes(viewModel.Filter.DataInicial.Value, viewModel.Filter.DataFinal.Value, cartaoCredito, cartaoDebito, dinheiro, requisicao, IdEmpresa);

            viewModel.Items = Mapper.Map<List<MovimentacaoVolumesListItemViewModel>>(dadosRetorno);

            return View(viewModel);
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosExpedicao.RelatorioPedidosExpedidos)]
        public ActionResult RelatorioPedidosExpedidos()
        {
            return View();
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosExpedicao.RelatorioPedidosExpedidos)]
        public ActionResult RelatorioPedidosExpedidosPageData(DataTableFilter<RelatorioPedidosExpedidosFilterViewModel> model)
        {
            var filter = Mapper.Map<DataTableFilter<RelatorioPedidosExpedidosFilter>>(model);

            filter.CustomFilter.IdEmpresa = IdEmpresa;

            IEnumerable<RelatorioPedidosExpedidosLinhaTabela> result = _expedicaoService.BuscarDadosPedidosExpedidos(filter, out int recordsFiltered, out int totalRecords);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = Mapper.Map<IEnumerable<RelatorioPedidosExpedidosListItemViewModel>>(result)
            });
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosExpedicao.MovimentacaoVolumes)]
        public ActionResult MovimentacaoVolumesDetalhes(DateTime dataInicial, DateTime dataFinal, long idGrupoCorredorArmazenagem, string status, string tipoPagamento, bool? requisicao)
        {
            bool? dinheiro = null, cartaoCredito = null, cartaoDebito = null;

            if (!tipoPagamento.NullOrEmpty())
            {
                if (tipoPagamento == "CC")
                {
                    cartaoCredito = true;
                }

                if (tipoPagamento == "CD")
                {
                    cartaoDebito = true;
                }

                if (tipoPagamento == "D")
                {
                    dinheiro = true;
                }
            }

            var dadosRetorno = _expedicaoService.BuscarDadosVolumes(dataInicial, dataFinal, idGrupoCorredorArmazenagem, status, cartaoCredito, cartaoDebito, dinheiro, requisicao, IdEmpresa, out string statusDescricao);

            var items = Mapper.Map<List<MovimentacaoVolumesDetalheListItemViewModel>>(dadosRetorno);

            var viewModel = new MovimentacaoVolumesDetalheViewModel
            {
                Status = statusDescricao,
                Items = items,
                Url = HttpContext.Request.Url.AbsoluteUri
            };

            return View(viewModel);
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosExpedicao.RelatorioPedidos)]
        public ActionResult RelatorioPedidos()
        {
            var model = new RelatorioPedidosViewModel
            {
                Filter = new RelatorioPedidosFilterViewModel()
                {
                    ListaStatus = new SelectList(
                    _uow.PedidoVendaStatusRepository.Todos().OrderBy(o => o.IdPedidoVendaStatus).Select(x => new SelectListItem
                    {
                        Value = x.IdPedidoVendaStatus.GetHashCode().ToString(),
                        Text = x.Descricao,
                    }), "Value", "Text"
                )
                }
            };

            model.Filter.DataInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00).AddDays(-10);
            model.Filter.DataFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosExpedicao.RelatorioPedidos)]
        public ActionResult RelatorioPedidosPageData(DataTableFilter<RelatorioPedidosFilterViewModel> model)
        {
            var filter = Mapper.Map<DataTableFilter<RelatorioPedidosFiltro>>(model);

            filter.CustomFilter.IdEmpresa = IdEmpresa;

            IEnumerable<RelatorioPedidosLinhaTabela> result = _expedicaoService.BuscarDadosPedidos(filter, out int recordsFiltered, out int totalRecords);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = Mapper.Map<IEnumerable<RelatorioPedidosListItemViewModel>>(result)
            });
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosExpedicao.RelatorioPedidos)]
        public ActionResult DetalhesPedidoVolume(long id)
        {
            var dadosRetorno = _expedicaoService.BuscarDadosPedidoVolume(id, IdEmpresa);

            return View(dadosRetorno);
        }

        [HttpGet]
        public ActionResult ConfirmarReimpressaoEtiquetaVolume(long idPedidoVendaVolume)
        {
            var pedidoVendaVolume = _uow.PedidoVendaVolumeRepository.GetById(idPedidoVendaVolume);

            return View(new ConfirmaReimpressaoVolumeViewModel
            {
                IdPedidoVendaVolume = pedidoVendaVolume.IdPedidoVendaVolume,
                NroPedido = pedidoVendaVolume.PedidoVenda.Pedido.NroPedido,
                Volume = pedidoVendaVolume.NroVolume
            });
        }

        [HttpPost]
        public JsonResult ReimprimirEtiquetaVolume(ReimpressaoEtiquetaVolumeViewModel viewModel)
        {
            try
            {
                ValidateModel(viewModel);

                var pedidoVendaVolume = _uow.PedidoVendaVolumeRepository.GetById(viewModel.IdPedidoVendaVolume);

                if (pedidoVendaVolume == null)
                {
                    throw new BusinessException("Volume não encontrado");
                }

                _etiquetaService.ImprimirEtiquetaVolume(pedidoVendaVolume, viewModel.IdImpressora);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Impressão enviada com sucesso."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (BusinessException exception)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = exception.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.CaixaRecusa.Cadastrar)]
        public ActionResult GerenciarVolumes()
        {
            var model = new GerenciarVolumeViewModel()
            {
                IdEmpresa = IdEmpresa
            };

            return View(model);
        }
       
        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Caixa.Cadastrar)]         
        public async Task<JsonResult> GerenciarVolumes(GerenciarVolumeRequisicao requisicao)
        {
            try
            {
                var listaVolumes = Mapper.Map<List<GerenciarVolumeItem>>(requisicao.ProdutosVolumes);

                var excedeuPeso = await _expedicaoService.GerenciarVolumes(requisicao.NroPedido, requisicao.IdPedidoVendaVolume, listaVolumes, IdEmpresa, requisicao.IdGrupoCorredorArmazenagem, IdUsuario).ConfigureAwait(false);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = excedeuPeso ? "Volumes alterados com sucesso. Atenção, os produtos excederam o peso máximo da caixa." : "Volumes alterados com sucesso."
                });
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = ex is BusinessException ? ex.Message : "Erro ao salvar volumes."
                });
            }
        }
    }
}
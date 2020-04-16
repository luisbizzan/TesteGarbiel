using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Model.Relatorios;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.ArmazenagemCtx;
using FWLog.Web.Backoffice.Models.CommonCtx;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class ArmazenagemController : BOBaseController
    {
        private readonly UnitOfWork _uow;
        private readonly RelatorioService _relatorioService;
        private readonly ILog _log;

        public ArmazenagemController(UnitOfWork uow, RelatorioService relatorioService, ILog log)
        {
            _uow = uow;
            _relatorioService = relatorioService;
            _log = log;
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioAtividadeEstoque)]
        public ActionResult RelatorioAtividadeEstoque()
        {
            var model = new RelatorioAtividadeEstoqueViewModel
            {
                Filter = new RelatorioAtividadeEstoqueFilterViewModel()
                {
                    ListaAtividadeEstoqueTipo = new SelectList(
                    _uow.AtividadeEstoqueTipoRepository.Todos().OrderBy(o => o.IdAtividadeEstoqueTipo).Select(x => new SelectListItem
                    {
                        Value = x.IdAtividadeEstoqueTipo.GetHashCode().ToString(),
                        Text = x.Descricao,
                    }), "Value", "Text"
                )
                }
            };
            
            return View(model); ;
        }

        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioAtividadeEstoque)]
        public ActionResult RelatorioAtividadeEstoquePageData(DataTableFilter<RelatorioAtividadeEstoqueFilterViewModel> model)
        {
            var filtro = Mapper.Map<DataTableFilter<AtividadeEstoqueListaFiltro>>(model);
            filtro.CustomFilter.IdEmpresa = IdEmpresa;

            var result = _uow.AtividadeEstoqueRepository.PesquisarPageData(filtro, out int registrosFiltrados, out int totalRegistros);

            var list = new List<RelatorioAtividadeEstoqueListItemViewModel>();
            
            List<UsuarioEmpresa> usuarios = _uow.UsuarioEmpresaRepository.ObterPorEmpresa(IdEmpresa);

            foreach (var item in result)
            {
                list.Add(new RelatorioAtividadeEstoqueListItemViewModel()
                {
                    CodigoEndereco = item.CodigoEndereco,
                    DataSolicitacao = item.DataSolicitacao.HasValue ? item.DataSolicitacao.Value.ToString("dd/MM/yyyy") : "",
                    DataExecucao = item.DataExecucao.HasValue ? item.DataExecucao.Value.ToString("dd/MM/yyyy"): "",
                    DescricaoProduto = item.DescricaoProduto,
                    ReferenciaProduto = item.ReferenciaProduto,
                    Finalizado = item.Finalizado ? "Sim" : "Não",
                    QuantidadeInicial = item.QuantidadeInicial.HasValue ? item.QuantidadeInicial.ToString() : "",
                    QuantidadeFinal = item.QuantidadeFinal.HasValue ? item.QuantidadeFinal.ToString() : "",
                    TipoAtividade = item.TipoAtividade,
                    UsuarioExecucao = usuarios.Where(x => x.UserId.Equals(item.UsuarioExecucao)).FirstOrDefault()?.PerfilUsuario.Nome,
                });
            }

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = list
            });
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioRastreabilidadeLote)]
        public ActionResult RelatorioRastreabilidadeLote()
        {
            return View();
        }

        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioRastreabilidadeLote)]
        public ActionResult RelatorioRastreabilidadeLotePageData(DataTableFilter<RelatorioRastreabilidadeLoteFilterViewModel> model)
        {
            var filtro = Mapper.Map<DataTableFilter<RastreabilidadeLoteListaFiltro>>(model);
            filtro.CustomFilter.IdEmpresa = IdEmpresa;

            var result = _uow.LoteProdutoRepository.PesquisarPorLoteOuProduto(filtro, out int registrosFiltrados, out int totalRegistros);

            var list = new List<RelatorioRastreabilidadeLoteListItemViewModel>();

            foreach (var item in result)
            {
                list.Add(new RelatorioRastreabilidadeLoteListItemViewModel()
                {
                    IdLote = item.IdLote,
                    Status = item.Status,
                    DataRecebimento = item.DataRecebimento.ToString("dd/MM/yyyy"),
                    DataConferencia = item.DataConferencia.HasValue ? item.DataConferencia.Value.ToString("dd/MM/yyyy") : "",
                    QuantidadeVolume = item.QuantidadeVolume.ToString(),
                    QuantidadePeca = item.QuantidadePeca.ToString()
                });
            }

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = list
            });
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioRastreabilidadeLote)]
        public ActionResult RelatorioRastreabilidadeLoteProduto(long idLote)
        {
            var model = new RelatorioRastreabilidadeLoteProdutoViewModel
            {
                Filter = new RelatorioRastreabilidadeLoteProdutoFilterViewModel()
            };

            model.Filter.IdLote = idLote;

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioRastreabilidadeLote)]
        public ActionResult RelatorioRastreabilidadeLoteProdutoPageData(DataTableFilter<RelatorioRastreabilidadeLoteProdutoFilterViewModel> model)
        {
            var filtro = Mapper.Map<DataTableFilter<RastreabilidadeLoteProdutoListaFiltro>>(model);
            filtro.CustomFilter.IdEmpresa = IdEmpresa;

            var result = _uow.LoteProdutoRepository.ConsultarPorLote(filtro, out int registrosFiltrados, out int totalRegistros);

            var list = new List<RelatorioRastreabilidadeLoteProdutoListItemViewModel>();

            foreach (var item in result)
            {
                list.Add(new RelatorioRastreabilidadeLoteProdutoListItemViewModel()
                {
                    IdProduto = item.IdProduto,
                    IdLote = item.IdLote,
                    ReferenciaProduto = item.ReferenciaProduto,
                    DescricaoProduto = item.DescricaoProduto,
                    QuantidadeRecebida = item.QuantidadeRecebida.ToString(),
                    Saldo = item.Saldo.ToString()
                });
            }

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = list
            });
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioRastreabilidadeLote)]
        public ActionResult RelatorioRastreabilidadeLoteMovimentacao(long idLote, long idProduto)
        {
            var model = new RelatorioRastreabilidadeLoteMovimentacaoViewModel
            {
                Filter = new RelatorioRastreabilidadeLoteMovimentacaoFilterViewModel()
                {
                    ListaLoteMovimentacaoTipo = new SelectList(
                    _uow.LoteMovimentacaoTipoRepository.Todos().OrderBy(o => o.IdLoteMovimentacaoTipo).Select(x => new SelectListItem
                    {
                        Value = x.IdLoteMovimentacaoTipo.GetHashCode().ToString(),
                        Text = x.Descricao,
                    }), "Value", "Text"
                )
                }
            };

            model.Filter.IdLote = idLote;
            model.Filter.IdProduto = idProduto;
            model.Filter.DescricaoProduto = _uow.ProdutoRepository.GetById(idProduto) != null ? _uow.ProdutoRepository.GetById(idProduto).Descricao : "";

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioRastreabilidadeLote)]
        public ActionResult RelatorioRastreabilidadeLoteMovimentacaoPageData(DataTableFilter<RelatorioRastreabilidadeLoteMovimentacaoFilterViewModel> model)
        {
            var filtro = Mapper.Map<DataTableFilter<RastreabilidadeLoteMovimentacaoListaFiltro>>(model);
            filtro.CustomFilter.IdEmpresa = IdEmpresa;

            var result = _uow.LoteMovimentacaoRepository.ConsultarPorLoteEProduto(filtro, out int registrosFiltrados, out int totalRegistros);

            var list = new List<RelatorioRastreabilidadeLoteMovimentacaoListItemViewModel>();
            List<UsuarioEmpresa> usuarios = _uow.UsuarioEmpresaRepository.ObterPorEmpresa(IdEmpresa);

            foreach (var item in result)
            {
                list.Add(new RelatorioRastreabilidadeLoteMovimentacaoListItemViewModel()
                {
                    IdProduto = item.IdProduto,
                    IdLote = item.IdLote,
                    ReferenciaProduto = item.ReferenciaProduto,
                    DescricaoProduto = item.DescricaoProduto,
                    UsuarioMovimentacao = usuarios.Where(x => x.UserId.Equals(item.IdUsuarioMovimentacao)).FirstOrDefault()?.PerfilUsuario.Nome,
                    Tipo = item.Tipo.ToString(),
                    Endereco = item.Endereco,
                    Quantidade = item.Quantidade.ToString(),
                    DataHora = item.DataHora.ToString("dd/MM/yyyy hh:mm:ss")
                });
            }

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = list
            });
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioLoteMovimentacao)]
        public ActionResult RelatorioLoteMovimentacao()
        {
            var model = new RelatorioLoteMovimentacaoViewModel
            {
                Filter = new RelatorioLoteMovimentacaoFilterViewModel()
                {
                    ListaLoteMovimentacaoTipo = new SelectList(
                    _uow.LoteMovimentacaoTipoRepository.Todos().OrderBy(o => o.IdLoteMovimentacaoTipo).Select(x => new SelectListItem
                    {
                        Value = x.IdLoteMovimentacaoTipo.GetHashCode().ToString(),
                        Text = x.Descricao,
                    }), "Value", "Text"
                )
                }
            };

            return View(model); ;
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioLoteMovimentacao)]
        public ActionResult RelatorioLoteMovimentacaoPageData(DataTableFilter<RelatorioLoteMovimentacaoFilterViewModel> model)
        {
            var filtro = Mapper.Map<DataTableFilter<LoteMovimentacaoListaFiltro>>(model);
            filtro.CustomFilter.IdEmpresa = IdEmpresa;

            var result = _uow.LoteMovimentacaoRepository.Consultar(filtro, out int registrosFiltrados, out int totalRegistros);

            var list = new List<RelatorioLoteMovimentacaoListItemViewModel>();
            List<UsuarioEmpresa> usuarios = _uow.UsuarioEmpresaRepository.ObterPorEmpresa(IdEmpresa);

            foreach (var item in result)
            {
                list.Add(new RelatorioLoteMovimentacaoListItemViewModel()
                {
                    IdProduto = item.IdProduto,
                    IdLote = item.IdLote,
                    ReferenciaProduto = item.ReferenciaProduto,
                    DescricaoProduto = item.DescricaoProduto,
                    UsuarioMovimentacao = usuarios.Where(x => x.UserId.Equals(item.IdUsuarioMovimentacao)).FirstOrDefault()?.PerfilUsuario.Nome,
                    Tipo = item.Tipo.ToString(),
                    Endereco = item.Endereco,
                    Quantidade = item.Quantidade.ToString(),
                    DataHora = item.DataHora.ToString("dd/MM/yyyy hh:mm:ss")
                });
            }

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = list
            });
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioTotalizacaoAlas)]
        public ActionResult RelatorioTotalizacaoAlas()
        {
            SetViewBags();

            return View();
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioTotalizacaoAlas)]
        public ActionResult RelatorioTotalizacaoAlasPageData(DataTableFilter<RelatorioTotalizacaoAlasFilterViewModel> model)
        {
            var filtro = Mapper.Map<DataTableFilter<RelatorioTotalizacaoAlasListaFiltro>>(model);
            filtro.CustomFilter.IdEmpresa = IdEmpresa;

            var listaEnderecoArmazenagem = _uow.EnderecoArmazenagemRepository
                .BuscarPorNivelEPontoArmazenagem(filtro.CustomFilter.IdNivelArmazenagem, filtro.CustomFilter.IdPontoArmazenagem, filtro.CustomFilter.IdEmpresa);

            filtro.CustomFilter.ListaEnderecoArmazenagem = listaEnderecoArmazenagem;

            var loteProdutoEnderecos = _uow.LoteProdutoEnderecoRepository.BuscarDados(filtro, out int totalRecordsFiltered, out int totalRecords);

            var list = new List<RelatorioTotalizacaoAlasListItemViewModel>();
            List<UsuarioEmpresa> usuarios = _uow.UsuarioEmpresaRepository.ObterPorEmpresa(IdEmpresa);

            loteProdutoEnderecos.ForEach(lpe =>
                list.Add(new RelatorioTotalizacaoAlasListItemViewModel
                {
                    NumeroCorredor = string.Concat("Corredor: ", lpe.Corredor.ToString("0#")),
                    CodigoEndereco = lpe.CodigoEndereco,
                    DataInstalacao = lpe.DataInstalacao != null ? lpe.DataInstalacao?.ToString("dd/MM/yyyy HH:mm:ss") : "-",
                    IdUsuarioInstalacao = usuarios.Where(x => x.UserId == lpe.IdUsuarioInstalacao).FirstOrDefault()?.PerfilUsuario.Nome ?? "-",
                    PesoProduto = lpe.PesoProduto != (decimal?)null ? lpe.PesoProduto?.ToString("n2") : "-",
                    QuantidadeProdutoPorEndereco = lpe.QuantidadeProdutoPorEndereco != (int?)null ? lpe.QuantidadeProdutoPorEndereco?.ToString() : "-",
                    ReferenciaProduto = lpe.ReferenciaProduto ?? "-",
                    IdLote = lpe.IdLote != null ? lpe.IdLote.ToString() : "-",
                    PesoTotalDeProduto = lpe.PesoTotalDeProduto != (decimal?)null ? lpe.PesoTotalDeProduto?.ToString("n2") : "-"
                })
            );

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = totalRecordsFiltered,
                Data = list
            });
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioTotalizacaoAlas)]
        public ActionResult DownloadRelatorioTotalPorAla(DownloadRelatorioTotalPorAlaViewModel viewModel)
        {
            ValidateModel(viewModel);

            var relatorioRequest = Mapper.Map<RelatorioTotalPorAlaRequest>(viewModel);
            relatorioRequest.IdEmpresa = IdEmpresa;
            relatorioRequest.NomeUsuarioRequisicao = LabelUsuario;
            byte[] relatorio = _relatorioService.GerarRelatorioTotalEnderecoPorAla(relatorioRequest);

            return File(relatorio, "application/pdf", "Relatório total por alas.pdf");
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioTotalizacaoAlas)]
        public JsonResult ImprimirRelatorioTotalPorAla(ImprimirRelatorioTotalPorAlaViewModel viewModel)
        {
            try
            {
                ValidateModel(viewModel);

                var request = Mapper.Map<ImprimirRelatorioTotalPorAlaRequest>(viewModel);

                request.IdEmpresa = IdEmpresa;
                request.NomeUsuarioRequisicao = LabelUsuario;

                _relatorioService.ImprimirRelatorioTotalEnderecoPorAla(request);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Impressão enviada com sucesso."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Ocorreu um erro na impressão."
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioPosicaoInventario)]
        public ActionResult RelatorioPosicaoInventario()
        {
            return View();
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioPosicaoInventario)]
        public JsonResult ValidarPesquisaRelatorioPosicaoInventario(long? idProduto, long? idNivelArmazenagem, long? idPontoArmazenagem)
        {
            if (idProduto == null && idNivelArmazenagem == null && idPontoArmazenagem == null)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Por favor, preencher pelo menos um dos filtros.",
                });
            }

            return Json(new AjaxGenericResultModel
            {
                Success = true
            });
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioPosicaoInventario)]
        public ActionResult RelatorioPosicaoInventarioPageData(DataTableFilter<RelatorioPosicaoInventarioFilterViewModel> model)
        {
            var list = new List<RelatorioPosicaoInventarioListItemViewModel>();

            if (!model.CustomFilter.IdNivelArmazenagem.HasValue && !model.CustomFilter.IdPontoArmazenagem.HasValue && !model.CustomFilter.IdProduto.HasValue)
            {
                return DataTableResult.FromModel(new DataTableResponseModel
                {
                    Draw = model.Draw,
                    RecordsTotal = 0,
                    RecordsFiltered = 0,
                    Data = list
                });
            }

            var filtro = Mapper.Map<DataTableFilter<RelatorioPosicaoInventarioListaFiltro>>(model);
            filtro.CustomFilter.IdEmpresa = IdEmpresa;

            var loteProdutoEnderecos = _uow.LoteProdutoEnderecoRepository.BuscarDadosPosicaoInventario(filtro, out int totalRecordsFiltered, out int totalRecords);

            loteProdutoEnderecos.OrderBy(x => x.Referencia).ThenBy(x => x.Codigo).ForEach(lpe => list.Add(new RelatorioPosicaoInventarioListItemViewModel
            {
                Codigo = lpe.Codigo,
                IdLote = lpe.IdLote.ToString(),
                QuantidadeProdutoPorEndereco = lpe.QuantidadeProdutoPorEndereco.ToString(),
                Referencia = string.Concat(lpe.Referencia, " - ", lpe.DescricaoProduto)

            }));

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = totalRecordsFiltered,
                Data = list
            });
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioPosicaoInventario)]
        public JsonResult ValidarDownloadOuImpressaoPosicaoInventario(long? idProduto, long? idNivelArmazenagem, long? idPontoArmazenagem)
        {
            if (idProduto == null && idNivelArmazenagem == null && idPontoArmazenagem == null)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Nenhum registro encontrado para download ou para impressão.",
                });
            }

            return Json(new AjaxGenericResultModel
            {
                Success = true
            });
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioPosicaoInventario)]
        public ActionResult DownloadRelatorioPosicaoInventario(DownloadRelatorioPosicaoInventarioViewModel viewModel)
        {
            var relatorioRequest = Mapper.Map<RelatorioPosicaoInventarioRequest>(viewModel);
            relatorioRequest.IdEmpresa = IdEmpresa;
            relatorioRequest.NomeUsuarioRequisicao = LabelUsuario;
            byte[] relatorio = _relatorioService.GerarRelatorioPosicaoParaInventario(relatorioRequest);

            return File(relatorio, "application/pdf", "Relatório - Posição para Inventário.pdf");
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioPosicaoInventario)]
        public JsonResult ImprimirRelatorioPosicaoInventario(ImprimirRelatorioPosicaoInventarioViewModel viewModel)
        {
            try
            {
                ValidateModel(viewModel);

                var request = Mapper.Map<ImprimirRelatorioPosicaoInventarioRequest>(viewModel);

                request.IdEmpresa = IdEmpresa;
                request.NomeUsuarioRequisicao = LabelUsuario;

                _relatorioService.ImprimirRelatorioPosicaoParaInventario(request);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Impressão enviada com sucesso."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Ocorreu um erro na impressão."
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioTotalizacaoLocalizacao)]
        public ActionResult RelatorioTotalizacaoLocalizacao()
        {
            SetViewBags();

            return View();
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioTotalizacaoLocalizacao)]
        public ActionResult RelatorioTotalizacaoLocalizacaoPageData(DataTableFilter<RelatorioTotalizacaoLocalizacaoFilterViewModel> model)
        {
            var filter = Mapper.Map<DataTableFilter<RelatorioTotalizacaoLocalizacaoFiltro>>(model);

            filter.CustomFilter.IdEmpresa = IdEmpresa;

            var result = _uow.LoteProdutoEnderecoRepository.BuscarDadosTotalizacaoLocalizacao(filter, out int recordsFiltered, out int totalRecords);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = Mapper.Map<IEnumerable<RelatorioTotalizacaoLocalizacaoListItemViewModel>>(result)
            });
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioTotalizacaoLocalizacao)]
        public ActionResult DownloadRelatorioRelatorioTotalizacaoLocalizacao(RelatorioTotalizacaoLocalizacaoFilterViewModel viewModel)
        {
            var relatorioRequest = Mapper.Map<RelatorioTotalizacaoLocalizacaoFiltro>(viewModel);

            relatorioRequest.IdEmpresa = IdEmpresa;

            var relatorio = _relatorioService.GerarRelatorioTotalizacaoLocalizacao(relatorioRequest, LabelUsuario);

            return File(relatorio, "application/pdf", "Relatório Totalização por Localização.pdf");
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioTotalizacaoLocalizacao)]
        public JsonResult ImprimirRelatorioRelatorioTotalizacaoLocalizacao(RelatorioTotalizacaoLocalizacaoFilterViewModel viewModel, long idImpressora)
        {
            try
            {
                ValidateModel(viewModel);

                var request = Mapper.Map<RelatorioTotalizacaoLocalizacaoFiltro>(viewModel);

                request.IdEmpresa = IdEmpresa;

                _relatorioService.ImprimirRelatorioTotalizacaoLocalizacao(request, idImpressora, LabelUsuario);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Impressão enviada com sucesso."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Ocorreu um erro na impressão."
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.ReltorioLogisticaCorredor)]
        public ActionResult RelatorioLogisticaCorredor()
        {
            SetViewBags();

            return View();
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.ReltorioLogisticaCorredor)]
        public ActionResult RelatorioLogisticaCorredorPageData(DataTableFilter<RelatorioLogisticaCorredorFilterViewModel> model)
        {
            var filtro = Mapper.Map<DataTableFilter<RelatorioLogisticaCorredorListaFiltro>>(model);

            filtro.CustomFilter.IdEmpresa = IdEmpresa;

            var produtos = _uow.LoteProdutoEnderecoRepository.BuscarDadosLogisticaCorredor(filtro, out int totalRecordsFiltered, out int totalRecords);

            var list = new List<RelatorioLogisticaCorredorListItemViewModel>();

            produtos.ForEach(lpe => list.Add(new RelatorioLogisticaCorredorListItemViewModel
            {
                Altura = lpe.Produto.Altura?.ToString("n2") ?? "-",
                Codigo = lpe.EnderecoArmazenagem.Codigo ?? "-",
                Referencia = lpe.Produto.Referencia ?? "-",
                Tipo = lpe.Produto.UnidadeMedida.Sigla ?? "-",
                Descricao = lpe.Produto.Descricao ?? "-",
                Saldo = lpe.ProdutoEstoque?.Saldo.ToString() ?? "-",
                Cubagem = lpe.Produto.MetroCubico.ToString() ?? "-",
                Largura = lpe.Produto.Largura?.ToString("n2") ?? "-",
                Comprimento = lpe.Produto.Comprimento?.ToString("n2") ?? "-",
                DtRepo = null,
                DuraDD = null,
                GiroDD = null,
                Giro6m = null,
                ItLoc = null
            }));

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = totalRecordsFiltered,
                Data = list
            });
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.ReltorioLogisticaCorredor)]
        public ActionResult DownloadRelatorioLogisticaCorredor(DownloadRelatorioLogisticaCorredorViewModel viewModel)
        {
            var relatorioRequest = Mapper.Map<RelatorioLogisticaCorredorRequest>(viewModel);

            relatorioRequest.IdEmpresa = IdEmpresa;
            relatorioRequest.NomeUsuarioRequisicao = LabelUsuario;
            byte[] relatorio = _relatorioService.GerarRelatorioLogisticaCorredor(relatorioRequest);

            return File(relatorio, "application/pdf", "Relatório - Logística por Corredor.pdf");
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.ReltorioLogisticaCorredor)]
        public JsonResult ImprimirRelatorioLogisticaCorredor(ImprimirRelatorioLogisticaCorredorViewModel viewModel)
        {
            try
            {
                ValidateModel(viewModel);

                var request = Mapper.Map<ImprimirRelatorioLogisticaCorredorRequest>(viewModel);

                request.IdEmpresa = IdEmpresa;
                request.NomeUsuarioRequisicao = LabelUsuario;

                _relatorioService.ImprimirRelatorioLogisticaCorredor(request);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Impressão enviada com sucesso."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Ocorreu um erro na impressão."
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
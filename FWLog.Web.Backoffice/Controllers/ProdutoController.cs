using AutoMapper;
using DartDigital.Library.Exceptions;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Model.Relatorios;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.ProdutoCtx;
using log4net;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class ProdutoController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ProdutoEstoqueService _produtoEstoqueService;
        private readonly RelatorioService _relatorioService;
        private readonly ILog _log;

        public ProdutoController(
            UnitOfWork unitOfWork,
            ProdutoEstoqueService produtoEstoqueService,
            RelatorioService relatorioService,
            ILog ilog)
        {
            _unitOfWork = unitOfWork;
            _produtoEstoqueService = produtoEstoqueService;
            _relatorioService = relatorioService;
            _log = ilog;
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Produto.Listar)]
        public ActionResult PageData(DataTableFilter<ProdutoListaFilterViewModel> model)
        {
            var filter = Mapper.Map<DataTableFilter<ProdutoListaFiltro>>(model);

            filter.CustomFilter.IdEmpresa = IdEmpresa;

            var produtoEstoque = _unitOfWork.ProdutoEstoqueRepository.ObterProdutoEstoquePorEmpresa(IdEmpresa);

            IEnumerable<ProdutoListaLinhaTabela> result = _unitOfWork.ProdutoRepository.FormatarDadosParaDataTable(filter, out int recordsFiltered, out int totalRecords, produtoEstoque);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = Mapper.Map<IEnumerable<ProdutoListaItemViewModel>>(result)
            });
        }

        public ActionResult Index()
        {
            SetViewBags();

            var model = new ProdutoListaViewModel();

            model.Filtros.ProdutoStatus = 2;

            return View(model);
        }

        [ApplicationAuthorize]
        public ActionResult SearchModal(long? id = null)
        {
            var lista = new[]
            {
                new SelectListItem { Value = "true", Text = "Ativo" },
                new SelectListItem { Value = "false", Text = "Inativo" }
            };

            var model = new ProdutoSearchModalViewModel()
            {
                Filter = new ProdutoSearchModalFilterViewModel()
                {
                    IdLote = id,
                    ListaStatus = new SelectList(lista, "Value", "Text")
                }
            };

            return View(model);
        }

        [ApplicationAuthorize]
        public ActionResult SearchModalPageData(DataTableFilter<ProdutoSearchModalFilterViewModel> model)
        {
            var filtro = Mapper.Map<DataTableFilter<ProdutoPesquisaModalFiltro>>(model);

            IEnumerable<ProdutoPesquisaModalListaLinhaTabela> result;

            int _registrosFiltrados;
            int _totalRegistros;

            if (filtro.CustomFilter.IdLote.HasValue)
            {
                result = _unitOfWork.LoteConferenciaRepository.BuscarLista(filtro, out int registrosFiltrados, out int totalRegistros);
                _registrosFiltrados = registrosFiltrados;
                _totalRegistros = totalRegistros;
            }
            else
            {
                result = _unitOfWork.ProdutoRepository.BuscarLista(filtro, out int registrosFiltrados, out int totalRegistros);
                _registrosFiltrados = registrosFiltrados;
                _totalRegistros = totalRegistros;
            }

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = _totalRegistros,
                RecordsFiltered = _registrosFiltrados,
                Data = Mapper.Map<IEnumerable<ProdutoSearchModalItemViewModel>>(result)
            });
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Produto.Visualizar)]
        public ActionResult DetalhesProduto(long id)
        {
            ProdutoEstoque produtoEstoque = _unitOfWork.ProdutoEstoqueRepository.ObterPorProdutoEmpresa(id, IdEmpresa);

            var viewModel = new ProdutoDetalhesViewModel
            {
                IdProduto = produtoEstoque.IdProduto,
                EnderecoArmazenagem = produtoEstoque.EnderecoArmazenagem?.Codigo,
                Comprimento = produtoEstoque.Produto.Comprimento?.ToString("n2"),
                Altura = produtoEstoque.Produto.Altura?.ToString("n2"),
                Descricao = produtoEstoque.Produto.Descricao,
                Largura = produtoEstoque.Produto.Largura?.ToString("n2"),
                Peso = produtoEstoque.Produto.PesoBruto.ToString("n2"),
                Referencia = produtoEstoque.Produto.Referencia,
                ImagemSrc = produtoEstoque.Produto.EnderecoImagem != "0" ? produtoEstoque.Produto.EnderecoImagem : null,
            };

            return View(viewModel);
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Produto.Editar)]
        public ActionResult EditarProduto(long id)
        {
            ProdutoEstoque produtoEstoque = _unitOfWork.ProdutoEstoqueRepository.ObterPorProdutoEmpresa(id, IdEmpresa);

            var viewModel = new ProdutoEditarViewModel
            {
                IdProduto = produtoEstoque.IdProduto
            };

            if (produtoEstoque.IdEnderecoArmazenagem != null)
            {
                EnderecoArmazenagem enderecoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.GetById(produtoEstoque.IdEnderecoArmazenagem.Value);

                viewModel.IdEnderecoArmazenagem = enderecoArmazenagem.IdEnderecoArmazenagem;
                viewModel.CodigoEnderecoArmazenagem = enderecoArmazenagem.Codigo;
                viewModel.IdNivelArmazenagem = enderecoArmazenagem.IdNivelArmazenagem;
                viewModel.DescricaoNivelArmazenagem = enderecoArmazenagem.NivelArmazenagem.Descricao;
                viewModel.IdPontoArmazenagem = enderecoArmazenagem.IdPontoArmazenagem;
                viewModel.DescricaoPontoArmazenagem = enderecoArmazenagem.PontoArmazenagem.Descricao;
            }

            return View(viewModel);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Produto.Editar)]
        public ActionResult EditarProduto(ProdutoEditarViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(viewModel);
                }

                _produtoEstoqueService.AtualizarOuInserirEnderecoArmazenagem(viewModel.IdProduto, viewModel.IdEnderecoArmazenagem.Value, IdEmpresa, IdUsuario);

                Notify.Success("Produto editado com sucesso.");

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                }, JsonRequestBehavior.DenyGet);
            }
            catch (BusinessException businessException)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = businessException.Message
                }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Produto.Listar)]
        public ActionResult DownloadRelatorioProduto(RelatorioProdutosRequest model)
        {
            model.IdEmpresa = IdEmpresa;
            model.NomeUsuario = LabelUsuario;

            byte[] relatorio = _relatorioService.GerarRelatorioProdutos(model);

            return File(relatorio, "application/pdf", "Relatório De Produtos.pdf");
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Produto.Listar)]
        public JsonResult ImprimirRelatorioProdutos(ImprimirRelatorioProdutosRequest model)
        {
            try
            {
                model.IdEmpresa = IdEmpresa;
                model.NomeUsuario = LabelUsuario;

                _relatorioService.ImprimirRelatorioProdutos(model);

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
        [ApplicationAuthorize(Permissions = Permissions.Produto.Listar)]
        public ActionResult DownloadDetalhesProduto(long id)
        {
            var produto = _unitOfWork.ProdutoRepository.GetById(id);

            var relatorioRequest = new DetalhesProdutoRequest
            {
                IdEmpresa = IdEmpresa,
                NomeUsuario = LabelUsuario,
                IdProduto = id,
                EnderecoImagem = produto.EnderecoImagem != "0" ? produto.EnderecoImagem : null,
            };

            byte[] relatorio = _relatorioService.GerarDetalhesProduto(relatorioRequest);

            return File(relatorio, "application/pdf", "Detalhes .pdf");
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Produto.Listar)]
        public JsonResult ImprimirDetalhesProduto(ProdutoImprimirDetalhes viewModel)
        {
            try
            {
                ValidateModel(viewModel);

                var relatorioRequest = new ImprimriDetalhesProdutoRequest
                {
                    IdEmpresa = IdEmpresa,
                    NomeUsuario = LabelUsuario,
                    IdProduto = viewModel.IdProduto,
                    IdImpressora = viewModel.IdImpressora
                };

                _relatorioService.ImprimirDetalhesProduto(relatorioRequest);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Impressão enviada com sucesso."
                }, JsonRequestBehavior.DenyGet);

            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Ocorreu um erro na impressão."
                }, JsonRequestBehavior.DenyGet);
            }
        }
    }
}
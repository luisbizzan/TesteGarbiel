using AutoMapper;
using DartDigital.Library.Exceptions;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Model.Etiquetas;
using FWLog.Services.Model.Relatorios;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.ProdutoCtx;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class ProdutoController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ProdutoEstoqueService _produtoEstoqueService;
        private readonly RelatorioService _relatorioService;
        private readonly EtiquetaService _etiquetaService;
        private readonly ILog _log;

        public ProdutoController(
            UnitOfWork unitOfWork,
            ProdutoEstoqueService produtoEstoqueService,
            RelatorioService relatorioService,
            EtiquetaService etiquetaService,
            ILog ilog)
        {
            _unitOfWork = unitOfWork;
            _produtoEstoqueService = produtoEstoqueService;
            _relatorioService = relatorioService;
            _etiquetaService = etiquetaService;
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
            ViewBag.ProdutoStatus = new SelectList(new List<SelectListItem>
                        {
                            new SelectListItem { Text = "Ativo", Value = "1"},
                            new SelectListItem { Text = "Inativo", Value = "0"},
                        }, "Value", "Text");

            ViewBag.LocacaoSaldo = new SelectList(new List<SelectListItem>
                        {
                            new SelectListItem { Text = "Sem Locação/Sem Saldo", Value = "0"},
                            new SelectListItem { Text = "Sem Locação/Com Saldo", Value = "1"},
                            new SelectListItem { Text = "Com Locação/Sem Saldo", Value = "2"},
                            new SelectListItem { Text = "Com Locação/Com Saldo", Value = "3"},
                        }, "Value", "Text");

            var model = new ProdutoListaViewModel();

            model.Filtros.ProdutoStatus = 1;

            model.Filtros.LocacaoSaldo = 0;

            return View(model);
        }

        [ApplicationAuthorize]
        public ActionResult SearchModal(long? id = null, long? idPedidoVendaVolume = null)
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
                    IdPedidoVendaVolume = idPedidoVendaVolume,
                    ListaStatus = new SelectList(lista, "Value", "Text"),
                    ExibirReferenciaProduto = idPedidoVendaVolume.HasValue ? true : false
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
            else if (filtro.CustomFilter.IdPedidoVendaVolume.HasValue)
            {
                result = _unitOfWork.PedidoVendaProdutoRepository.BuscarLista(filtro, out int registrosFiltrados, out int totalRegistros);
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
                Cubagem = produtoEstoque.Produto.MetroCubico?.ToString("n2"),
                Unidade = produtoEstoque.Produto.UnidadeMedida.Sigla,
                Multiplo = produtoEstoque.Produto.MultiploVenda.ToString(),
                Status = produtoEstoque.ProdutoEstoqueStatus.Descricao,
                Saldo = produtoEstoque.Saldo.ToString(),
                ImagemSrc = produtoEstoque.Produto.EnderecoImagem != "0" ? produtoEstoque.Produto.EnderecoImagem : null,
            };

            viewModel.ListaLocaisArmazenagem = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorProduto(produtoEstoque.IdProduto, IdEmpresa).Select(lpe => new ProdutoDetalhesLocalArmazenagemViewModel
            {
                PontoArmazenagemDescricao = lpe.EnderecoArmazenagem.PontoArmazenagem.Descricao,
                NivelArmazenagemDescricao = lpe.EnderecoArmazenagem.NivelArmazenagem.Descricao,
                IdLote = lpe.IdLote,
                FornecedorNomeFantasia = lpe.Lote?.NotaFiscal?.Fornecedor?.NomeFantasia,
                EnderecoArmazenagemCodigo = lpe.EnderecoArmazenagem.Codigo,
                Quantidade = lpe.Quantidade
            }).ToList();

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

        [HttpGet]
        public ActionResult ConfirmarImpressao(long idEnderecoArmazenagem, long idProduto)
        {
            var endereco = _unitOfWork.EnderecoArmazenagemRepository.GetById(idEnderecoArmazenagem);
            var produto = _unitOfWork.ProdutoRepository.GetById(idProduto);

            return View(new ProdutoConfirmaImpressaoViewModel
            {
                IdEnderecoArmazenagem = endereco.IdEnderecoArmazenagem,
                Codigo = endereco.Codigo,
                IdProduto = produto.IdProduto,
                DescricaoProduto = produto.Descricao,
                Referencia = produto.Referencia
            });
        }

        [HttpPost]
        public JsonResult ImprimirEtiqueta(ProdutoImpressaoViewModel viewModel)
        {
            try
            {
                ValidateModel(viewModel);

                if (viewModel.TipoImpressaoEtiqueta == TipoImpressaoEtiqueta.PICKING)
                {
                    var request = new ImprimirEtiquetaPickingRequest
                    {
                        IdEnderecoArmazenagem = viewModel.IdEnderecoArmazenagem,
                        IdProduto = viewModel.IdProduto,
                        QuantidadeEtiquetas = 1,
                        IdImpressora = viewModel.IdImpressora
                    };

                    _etiquetaService.ImprimirEtiquetaPicking(request);
                }
                else
                {
                    _etiquetaService.ImprimirEtiquetaFilete(viewModel.IdProduto, viewModel.IdEnderecoArmazenagem, viewModel.IdImpressora);
                }

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Impressão enviada com sucesso."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                if (exception is BusinessException)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = exception.Message
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    _log.Error(exception.Message, exception);

                    throw;
                }
            }
        }
    }
}
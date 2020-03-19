using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.ProdutoCtx;
using System.Collections.Generic;
using System;
using System.Web.Mvc;
using FWLog.Services.Services;
using FWLog.Data.EnumsAndConsts;

namespace FWLog.Web.Backoffice.Controllers
{
    public class ProdutoController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ProdutoEstoqueService _produtoEstoqueService;
        private readonly ApplicationLogService _applicationLogService;

        public ProdutoController(UnitOfWork unitOfWork, ProdutoEstoqueService produtoEstoqueService, ApplicationLogService applicationLogService)
        {
            _unitOfWork = unitOfWork;
            _produtoEstoqueService = produtoEstoqueService;
            _applicationLogService = applicationLogService;
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Produto.Listar)]
        public ActionResult PageData(DataTableFilter<ProdutoListaFilterViewModel> model)
        {
            var filter = Mapper.Map<DataTableFilter<ProdutoListaFiltro>>(model);

            filter.CustomFilter.IdEmpresa = IdEmpresa;

            IEnumerable<ProdutoListaLinhaTabela> result = _unitOfWork.ProdutoRepository.ObterDadosParaDataTable(filter, out int recordsFiltered, out int totalRecords);

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

            return View();
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

                ProdutoEstoque produtoEstoque = _unitOfWork.ProdutoEstoqueRepository.ConsultarPorProduto(viewModel.IdProduto, IdEmpresa);

                if (produtoEstoque == null)
                {
                    Notify.Error("Produto não localizado!");
                }

                _produtoEstoqueService.AtualizarOuInserirEnderecoArmazenagem(produtoEstoque, viewModel.IdEnderecoArmazenagem);

                Notify.Success("Endereço de Armazenagem editado com sucesso.");
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                _applicationLogService.Error(ApplicationEnum.BackOffice, e);
                Notify.Error("Algo inesperado ocorreu!");
                return RedirectToAction("Index");
            }
        }
    }
}
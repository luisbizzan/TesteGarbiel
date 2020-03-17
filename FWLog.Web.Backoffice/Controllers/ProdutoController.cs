using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.ProdutoCtx;
using System.Collections.Generic;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class ProdutoController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;

        public ProdutoController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
    }
}
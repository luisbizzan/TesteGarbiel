﻿using AutoMapper;
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
    public class ProdutoController : Controller
    {
        private readonly UnitOfWork _uow;

        public ProdutoController(UnitOfWork uow)
        {
            _uow = uow;
        }

        // GET: Fornecedor
        public ActionResult Index()
        {
            return View();
        }

        [ApplicationAuthorize]
        public ActionResult SearchModal()
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
                    ListaStatus = new SelectList(lista, "Value", "Text")
                }
            };

            return View(model);
        }

        [ApplicationAuthorize]
        public ActionResult SearchModalPageData(DataTableFilter<ProdutoSearchModalFilterViewModel> model)
        {
            var filtro = Mapper.Map<DataTableFilter<ProdutoPesquisaModalFiltro>>(model);

            IEnumerable<ProdutoPesquisaModalListaLinhaTabela> result = _uow.ProdutoRepository.BuscarLista(filtro, out int registrosFiltrados, out int totalRegistros);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = Mapper.Map<IEnumerable<ProdutoSearchModalItemViewModel>>(result)
            });
        }
    }
}
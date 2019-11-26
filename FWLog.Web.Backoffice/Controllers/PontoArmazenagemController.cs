using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.PontoArmazenagemCtx;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using FWLog.Data.Models;
using FWLog.Data.Models.FilterCtx;
using AutoMapper;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Data.Models.DataTablesCtx;

namespace FWLog.Web.Backoffice.Controllers
{
    public class PontoArmazenagemController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;

        public PontoArmazenagemController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.PontoArmazenagem.Listar)]
        public ActionResult Index()
        {
            var viewModel = new PontoArmazenagemListaViewModel
            {
                NiveisArmazenagem = new SelectList(_unitOfWork.NivelArmazenagemRepository.RetornarAtivos().Select(x => new SelectListItem
                {
                    Value = x.IdNivelArmazenagem.ToString(),
                    Text = x.Descricao,
                }), "Value", "Text"),
                TiposArmazenagem = new SelectList(_unitOfWork.TipoArmazenagemRepository.RetornarTodos().Select(x => new SelectListItem
                {
                    Value = (x.IdTipoArmazenagem.GetHashCode()).ToString(),
                    Text = x.Descricao,
                }), "Value", "Text"),
                TiposMovimentacao = new SelectList(_unitOfWork.TipoMovimentacaoRepository.RetornarTodos().Select(x => new SelectListItem
                {
                    Value = (x.IdTipoMovimentacao.GetHashCode()).ToString(),
                    Text = x.Descricao,
                }), "Value", "Text"),
                Status = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Text = "Todos", Value = ""},
                    new SelectListItem { Text = "Ativo", Value = "1"},
                    new SelectListItem { Text = "Inativo", Value = "2"}
                }, "Value", "Text")
            };

            return View(viewModel);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.PontoArmazenagem.Listar)]
        public ActionResult PageData(DataTableFilter<PontoArmazenagemListaFilterViewModel> model)
        {
            //var filtro = new DataTableFilter<PontoArmazenagemListaFiltro>
            //{
            //    CustomFilter = new PontoArmazenagemListaFiltro
            //    {
            //        Descricao = model.CustomFilter.Descricao,
            //        IdEmpresa = IdEmpresa,
            //        IdNivelArmazenagem = model.CustomFilter.IdNivelArmazenagem,
            //        IdTipoArmazenagem = (TipoArmazenagemEnum?)model.CustomFilter.IdTipoArmazenagem,
            //        IdTipoMovimentacao = (TipoMovimentacaoEnum?)model.CustomFilter.IdTipoMovimentacao,
            //        Status = model.CustomFilter.Status
            //    }
            //};

            var filtro = Mapper.Map<DataTableFilter<PontoArmazenagemListaFiltro>>(model);
            filtro.CustomFilter.IdEmpresa = IdEmpresa;

            IEnumerable<PontoArmazenagemListaLinhaTabela> result = _unitOfWork.PontoArmazenagemRepository.BuscarLista(filtro, out int registrosFiltrados, out int totalRegistros);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = Mapper.Map<IEnumerable<PontoArmazenagemListaItemViewModel>>(result)
            });
        }
    }
}
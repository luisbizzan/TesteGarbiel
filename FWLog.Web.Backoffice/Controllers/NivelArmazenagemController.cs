using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.EnumsAndConsts.LOVs;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.NivelArmazenagemCtx;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class NivelArmazenagemController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly NivelArmazenagemService _nivelArmazenagemService;

        private SelectList Status
        {
            get
            {
                if (status == null)
                {
                    status = new SelectList(new NaoSimLOV().Items, "Value", "Text");
                }

                return status;
            }
        }
        private SelectList status;


        private void setViewBags()
        {
            ViewBag.Status = Status;
        }

        public NivelArmazenagemController(
            UnitOfWork unitOfWork,
            NivelArmazenagemService nivelArmazenagemService)
        {
            _unitOfWork = unitOfWork;
            _nivelArmazenagemService = nivelArmazenagemService;
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.NivelArmazenagem.List)]
        public ActionResult Index()
        {
            setViewBags();

            return View(new NivelArmazenagemListViewModel());
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.NivelArmazenagem.List)]
        public ActionResult PageData(DataTableFilter<NivelArmazenagemFilterViewModel> model)
        {
            var filter = Mapper.Map<DataTableFilter<NivelArmazenagemFilter>>(model);

            IEnumerable<NivelArmazenagemTableRow> result = _unitOfWork.NivelArmazenagemRepository.SearchForDataTable(filter, out int recordsFiltered, out int totalRecords);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = Mapper.Map<IEnumerable<NivelArmazenagemListItemViewModel>>(result)
            });
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.NivelArmazenagem.Create)]
        public ActionResult Create()
        {
            setViewBags();

            return View();
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.NivelArmazenagem.Create)]
        public ActionResult Create(NivelArmazenagemCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                setViewBags();

                return View(model);
            }

            var entity = new NivelArmazenagem { Ativo = model.Ativo, Descricao = model.Descricao, IdEmpresa = IdEmpresa };

            _nivelArmazenagemService.Add(entity);

            Notify.Success(Resources.CommonStrings.RegisterCreatedSuccessMessage);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.NivelArmazenagem.Edit)]
        public ActionResult Edit(int id)
        {
            setViewBags();

            NivelArmazenagem nivelArmazenagem = _unitOfWork.NivelArmazenagemRepository.GetById(id);

            if (nivelArmazenagem == null)
            {
                throw new HttpException(404, "Not found");
            }

            var model = Mapper.Map<NivelArmazenagemCreateViewModel>(nivelArmazenagem);

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.NivelArmazenagem.Edit)]
        public ActionResult Edit(NivelArmazenagemCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                setViewBags();

                return View(model);
            }

            var entity = new NivelArmazenagem { IdNivelArmazenagem = model.IdNivelArmazenagem, Ativo = model.Ativo, Descricao = model.Descricao, IdEmpresa = model.IdEmpresa };

            _nivelArmazenagemService.Edit(entity);

            Notify.Success(Resources.CommonStrings.RegisterEditedSuccessMessage);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.NivelArmazenagem.Delete)]
        public JsonResult AjaxDelete(int id)
        {
            try
            {
                _nivelArmazenagemService.Delete(_unitOfWork.NivelArmazenagemRepository.GetById(id));

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = Resources.CommonStrings.RegisterDeletedSuccessMessage
                }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = Resources.CommonStrings.RegisterHasRelationshipsErrorMessage
                }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        [ApplicationAuthorize]
        public ActionResult PesquisaModal()
        {
            var viewModel = new NivelArmazenagemPesquisaModalViewModel
            {
                Status = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Text = "Ativo", Value = "1"},
                    new SelectListItem { Text = "Inativo", Value = "0"}
                }, "Value", "Text")
            };

            return View(viewModel);
        }

        [HttpPost]
        [ApplicationAuthorize]
        public ActionResult PesquisaModalDadosLista(DataTableFilter<NivelArmazenagemPesquisaModalFiltroViewModel> model)
        {
            var filtros = Mapper.Map<DataTableFilter<NivelArmazenagemPesquisaModalFiltro>>(model);
            filtros.CustomFilter.IdEmpresa = IdEmpresa;

            IEnumerable<NivelArmazenagemPesquisaModalListaLinhaTabela> result = _unitOfWork.NivelArmazenagemRepository.BuscarListaModal(filtros, out int registrosFiltrados, out int totalRegistros);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = Mapper.Map<IEnumerable<NivelArmazenagemPesquisaModalItemViewModel>>(result)
            });
        }
    }
}

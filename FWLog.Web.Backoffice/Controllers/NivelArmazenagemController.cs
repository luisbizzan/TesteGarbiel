using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Services;
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
        private readonly UnitOfWork _uow;
        private NivelArmazenagemService _nivelArmazenagemService;

        public NivelArmazenagemController(UnitOfWork uow, NivelArmazenagemService nivelArmazenagemService)
        {
            _uow = uow;
            _nivelArmazenagemService = nivelArmazenagemService;
        }

        [ApplicationAuthorize(Permissions = Permissions.NivelArmazenagem.List)]
        public ActionResult Index()
        {
            return View(new NivelArmazenagemListViewModel());
        }

        [ApplicationAuthorize(Permissions = Permissions.NivelArmazenagem.List)]
        public ActionResult PageData(DataTableFilter<NivelArmazenagemFilterViewModel> model)
        {
            int recordsFiltered, totalRecords;
            var filter = Mapper.Map<DataTableFilter<NivelArmazenagemFilter>>(model);
            IEnumerable<NivelArmazenagemTableRow> result = _uow.NivelArmazenagemRepository.SearchForDataTable(filter, out recordsFiltered, out totalRecords);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = Mapper.Map<IEnumerable<NivelArmazenagemListItemViewModel>>(result)
            });
        }

        [ApplicationAuthorize(Permissions = Permissions.NivelArmazenagem.Create)]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.NivelArmazenagem.Create)]
        public ActionResult Create(NivelArmazenagemCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var nivelArmazenagem = Mapper.Map<NivelArmazenagem>(model);

            _nivelArmazenagemService.Add(nivelArmazenagem);

            Notify.Success(Resources.CommonStrings.RegisterCreatedSuccessMessage);
            return RedirectToAction("Index");
        }

        [ApplicationAuthorize(Permissions = Permissions.NivelArmazenagem.Edit)]
        public ActionResult Edit(int id)
        {
            NivelArmazenagem nivelArmazenagem = _uow.NivelArmazenagemRepository.GetById(id);

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
                return View(model);
            }

            NivelArmazenagem nivelArmazenagem = Mapper.Map<NivelArmazenagem>(model);

            _nivelArmazenagemService.Edit(nivelArmazenagem);

            Notify.Success(Resources.CommonStrings.RegisterEditedSuccessMessage);
            return RedirectToAction("Index");
        }

        [ApplicationAuthorize(Permissions = Permissions.NivelArmazenagem.List)]
        public ActionResult Details(int id)
        {
            NivelArmazenagem nivelArmazenagem = _uow.NivelArmazenagemRepository.GetById(id);

            if (nivelArmazenagem == null)
            {
                throw new HttpException(404, "Not found");
            }

            var model = Mapper.Map<NivelArmazenagemDetailsViewModel>(nivelArmazenagem);

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.NivelArmazenagem.Delete)]
        public JsonResult AjaxDelete(int id)
        {
            try
            {
                _nivelArmazenagemService.Delete(_uow.NivelArmazenagemRepository.GetById(id));

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

    }
}

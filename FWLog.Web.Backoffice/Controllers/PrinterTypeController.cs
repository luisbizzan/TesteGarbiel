using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.PrinterTypeCtx;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class PrinterTypeController : BOBaseController
    {
        private UnitOfWork _uow;
        private PrinterTypeService _printerTypeService;

        public PrinterTypeController(UnitOfWork uow, PrinterTypeService printerTypeService)
        {
            _uow = uow;
            _printerTypeService = printerTypeService;
        }

        [ApplicationAuthorize(Permissions = Permissions.PrinterType.List)]
        public ActionResult Index()
        {
            return View(new PrinterTypeListViewModel());
        }

        [ApplicationAuthorize(Permissions = Permissions.PrinterType.List)]
        public ActionResult PageData(DataTableFilter<PrinterTypeFilterViewModel> model)
        {
            var filter = Mapper.Map<DataTableFilter<PrinterTypeFilter>>(model);
            IEnumerable<PrinterTypeTableRow> result = _uow.BOPrinterTypeRepository.SearchForDataTable(filter, out int recordsFiltered, out int totalRecords);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = Mapper.Map<IEnumerable<PrinterTypeListItemViewModel>>(result)
            });
        }

        [ApplicationAuthorize(Permissions = Permissions.PrinterType.Create)]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.PrinterType.Create)]
        public ActionResult Create(PrinterTypeCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var printerType = Mapper.Map<PrinterType>(model);

            _printerTypeService.Add(printerType);

            Notify.Success(Resources.CommonStrings.RegisterCreatedSuccessMessage);
            return RedirectToAction("Index");
        }

        [ApplicationAuthorize(Permissions = Permissions.PrinterType.Edit)]
        public ActionResult Edit(int id)
        {
            PrinterType printerType = _uow.BOPrinterTypeRepository.GetById(id);

            if (printerType == null)
            {
                throw new HttpException(404, "Not found");
            }

            var model = Mapper.Map<PrinterTypeCreateViewModel>(printerType);

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.PrinterType.Edit)]
        public ActionResult Edit(PrinterTypeCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            PrinterType printerType = Mapper.Map<PrinterType>(model);

            _printerTypeService.Edit(printerType);

            Notify.Success(Resources.CommonStrings.RegisterEditedSuccessMessage);
            return RedirectToAction("Index");
        }

        [ApplicationAuthorize(Permissions = Permissions.PrinterType.List)]
        public ActionResult Details(int id)
        {
            PrinterType printerType = _uow.BOPrinterTypeRepository.GetById(id);

            if (printerType == null)
            {
                throw new HttpException(404, "Not found");
            }

            var model = Mapper.Map<PrinterTypeDetailsViewModel>(printerType);

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.PrinterType.Delete)]
        public JsonResult AjaxDelete(int id)
        {
            try
            {
                _printerTypeService.Delete(_uow.BOPrinterTypeRepository.GetById(id));

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

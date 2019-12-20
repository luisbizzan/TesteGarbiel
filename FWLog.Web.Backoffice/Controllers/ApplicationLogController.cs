using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.ApplicationLogCtx;
using FWLog.Web.Backoffice.Models.CommonCtx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class ApplicationLogController : BOBaseController
    {
        UnitOfWork _uow;

        ApplicationLogService _applicationLogService;

        public ApplicationLogController(UnitOfWork uow, ApplicationLogService applicationLogService)
        {
            _uow = uow;
            _applicationLogService = applicationLogService;
        }

        [ApplicationAuthorize(Permissions = Permissions.ApplicationLog.List)]
        public ActionResult Index()
        {
            var filter = new ApplicationLogFilterViewModel
            {
                LevelOptions = GetApplicationLogLevelSelectList()
            };

            return View(new ApplicationLogListViewModel { Filter = filter });
        }

        [ApplicationAuthorize(Permissions = Permissions.ApplicationLog.List)]
        public DataTableResult PageData(DataTableFilter<ApplicationLogFilterViewModel> model)
        {
            int recordsFiltered, totalRecords;
            var filter = Mapper.Map<DataTableFilter<ApplicationLogFilter>>(model);
            IEnumerable<ApplicationLogTableRow> result = _uow.ApplicationLogRepository.SearchForDataTable(filter, out recordsFiltered, out totalRecords);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = Mapper.Map<IEnumerable<ApplicationLogListItemViewModel>>(result)
            });
        }

        [ApplicationAuthorize(Permissions = Permissions.ApplicationLog.List)]
        public ActionResult Details(int id)
        {
            ApplicationLog log = _uow.ApplicationLogRepository.GetById(id);

            if (log == null)
            {
                throw new HttpException(404, "Not found");
            }

            var model = Mapper.Map<ApplicationLogDetailsViewModel>(log);

            return View(model);
        }

        private SelectList GetApplicationLogLevelSelectList()
        {
            return new SelectList(
                ApplicationLogLevel.GetAll().Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Value
                }), "Value", "Text"
            );

        }

        [HttpPost]
        public ActionResult LogBadAjaxCall(string url)
        {
            try
            {
                _applicationLogService.Add(new ApplicationLog
                {
                    Created = DateTime.UtcNow,
                    Level = "ERROR",
                    Message = "Erro ao fazer chamada ajax para a URL: " + url,
                    Exception = "",
                    IdApplication = ApplicationEnum.BackOffice.GetHashCode()
                });

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Criado log de erro com sucesso"
                }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception e)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Não foi possível criar log de erro para "
                }, JsonRequestBehavior.DenyGet);
            }
        }
    }
}

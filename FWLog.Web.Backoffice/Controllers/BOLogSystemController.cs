using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Models.GeneralCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.EnumsAndConsts;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.BOLogSystemCtx;
using FWLog.Web.Backoffice.Models.CommonCtx;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class BOLogSystemController : BOBaseController
    {
        UnitOfWork _uow;
        BOLogSystemService _service;

        public BOLogSystemController(UnitOfWork uow, BOLogSystemService service)
        {
            _uow = uow;
            _service = service;
        }

        [ApplicationAuthorize(Permissions = Permissions.BOLogSystem.List)]
        public ActionResult Index()
        {
            var model = new BOLogSystemListViewModel();
            IEnumerable<LogEntity> entities = _uow.BOLogSystemRepository.GetLogEntities().OrderBy(x => x.TranslatedName);

            model.Filter = new BOLogSystemFilterViewModel
            {
                ActionTypeOptions = Mapper.Map<SelectList>(ActionTypeNames.GetAll()),
                EntityOptions = Mapper.Map<SelectList>(entities)
            };

            return View(model);
        }

        [ApplicationAuthorize(Permissions = Permissions.BOLogSystem.List)]
        public ActionResult PageData(DataTableFilter<BOLogSystemFilterViewModel> model)
        {
            int recordsFiltered, totalRecords;
            var filter = Mapper.Map<DataTableFilter<BOLogSystemFilter>>(model);
            IEnumerable<BOLogSystemTableRow> result = _uow.BOLogSystemRepository.SearchForDataTable(filter, out recordsFiltered, out totalRecords);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = Mapper.Map<IEnumerable<BOLogSystemListItemViewModel>>(result)
            });
        }

        [ApplicationAuthorize(Permissions = Permissions.BOLogSystem.List)]
        public ActionResult Details(int id, int page = 1)
        {
            BOLogSystemDetails details = _uow.BOLogSystemRepository.GetDetailsById(id);

            if (details == null)
            {
                throw new HttpException(404, "Not found");
            }

            BOLogSystemDetailsViewModel model = Mapper.Map<BOLogSystemDetailsViewModel>(details);

            return View(model);
        }
    }
}

using AutoMapper;
using ExtensionMethods;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.BOPrinterCtx;
using FWLog.Web.Backoffice.Models.CommonCtx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class BOPrinterController : BOBaseController
    {
        private readonly UnitOfWork _uow;
        private readonly BOLogSystemService _boLogSystemService;

        private SelectList PrinterTypes
        {
            get
            {
                if (printertypes == null)
                {
                    printertypes = new SelectList(
                        _uow.BOPrinterTypeRepository.GetAll().Select(x => new SelectListItem
                        {
                            Value = x.Id.ToString(),
                            Text = x.Name,
                        }).ToList(), "Value", "Text");
                }

                return printertypes;
            }
        }
        private SelectList printertypes;

        private SelectList Companies
        {
            get
            {
                if (companies == null)
                {
                    companies = new SelectList(
                        _uow.CompanyRepository.GetAll().Select(x => new SelectListItem
                        {
                            Value = x.CompanyId.ToString(),
                            Text = x.CompanyName,
                        }).ToList(), "Value", "Text");
                }

                return companies;
            }
        }
        private SelectList companies;

        private void setViewBags()
        {
            ViewBag.PrinterTypes = PrinterTypes;
            ViewBag.Companies = Companies;
        }

        public BOPrinterController(UnitOfWork uow, BOLogSystemService boLogSystemService)
        {
            _uow = uow;
            _boLogSystemService = boLogSystemService;
        }

        [ApplicationAuthorize(Permissions = Permissions.Role.List)]
        public ActionResult Index()
        {
            return View(new BOPrinterListViewModel());
        }

        [ApplicationAuthorize(Permissions = Permissions.Role.List)]
        public ActionResult PageData(DataTableFilter<BOPrinterFilterViewModel> model)
        {
            IQueryable<Printer> all = _uow.BOPrinterRepository.All();

            IEnumerable<Printer> query = all.WhereIf(!string.IsNullOrEmpty(model.CustomFilter.Name), x => x.Name.Contains(model.CustomFilter.Name));

            int totalRecords = all.Count();
            int recordsFiltered = query.Count();

            List<Printer> result = query.PaginationResult(model);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = Mapper.Map<IEnumerable<BOPrinterListItemViewModel>>(result)
            });
        }

        [ApplicationAuthorize(Permissions = Permissions.Role.Create)]
        public ActionResult Create()
        {
            setViewBags();

            return View(new BOPrinterCreateViewModel());
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Role.Create)]
        public ActionResult Create(BOPrinterCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                setViewBags();

                return View(model);
            }

            try
            {
                var entity = new Printer() { Name = model.Name, IP = model.IP, CompanyId = model.CompanyId, PrinterTypeId = model.PrinterTypeId };

                _uow.BOPrinterRepository.Add(entity);

                _uow.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //var userInfo = new BackOfficeUserInfo();
            //_boLogSystemService.Add(new BOLogSystemCreation
            //{
            //    ActionType = ActionTypeNames.Add,
            //    IP = userInfo.IP,
            //    UserId = userInfo.UserId,
            //    EntityName = nameof(Printer),
            //    NewEntity = new AspNetRolesLogSerializeModel(model.Name)
            //});

            Notify.Success(Resources.CommonStrings.RegisterCreatedSuccessMessage);
            return RedirectToAction("Index");
        }

        [ApplicationAuthorize(Permissions = Permissions.Role.List)]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var entity = _uow.BOPrinterRepository.All().FirstOrDefault(x => x.Id == id);

            if (entity == null)
            {
                throw new HttpException(404, "Not found");
            }

            var model = new BOPrinterDetailsViewModel() { Name = entity.Name, Company = entity.Company.CompanyName, PrinterType = entity.PrinterType.Name, IP = entity.IP };

            return View(model);
        }

        [ApplicationAuthorize(Permissions = Permissions.Role.Edit)]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var entity = _uow.BOPrinterRepository.All().FirstOrDefault(x => x.Id == id);

            if (entity == null)
            {
                throw new HttpException(404, "Not found");
            }

            var model = new BOPrinterCreateViewModel() { Name = entity.Name, CompanyId = entity.CompanyId, IP = entity.IP, PrinterTypeId = entity.PrinterTypeId };

            setViewBags();

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Role.Edit)]
        public ActionResult Edit(BOPrinterCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                setViewBags();

                return View(model);
            }

            try
            {
                var entity = _uow.BOPrinterRepository.All().FirstOrDefault(x => x.Id == model.Id);

                if (entity == null)
                {
                    throw new HttpException(404, "Not found");
                }

                entity.Name = model.Name;
                entity.PrinterTypeId = model.PrinterTypeId;
                entity.CompanyId = model.CompanyId;
                entity.IP = model.IP;

                _uow.BOPrinterRepository.Update(entity);

                _uow.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }

            //var userInfo = new BackOfficeUserInfo();
            //_boLogSystemService.Add(new BOLogSystemCreation
            //{
            //    ActionType = ActionTypeNames.Edit,
            //    IP = userInfo.IP,
            //    UserId = userInfo.UserId,
            //    EntityName = nameof(Printer),
            //    OldEntity = entity,
            //    //NewEntity = new AspNetRolesLogSerializeModel(role.Name)
            //});

            Notify.Success(Resources.CommonStrings.RegisterEditedSuccessMessage);
            return RedirectToAction("Index");
        }

        public ActionResult Selecionar()
        {
            var listaImpressoras = new List<BOPrinterSelecionarImpressoraViewModel>();

            listaImpressoras.Add(new BOPrinterSelecionarImpressoraViewModel
            {
                Id = 1,
                Nome = "Impressora 01"
            });

            listaImpressoras.Add(new BOPrinterSelecionarImpressoraViewModel
            {
                Id = 2,
                Nome = "Impressora 02"
            });

            listaImpressoras.Add(new BOPrinterSelecionarImpressoraViewModel
            {
                Id = 3,
                Nome = "Impressora 03"
            });

            var viewModel = new BOPrinterSelecionarViewModel
            {
                Impressoras = listaImpressoras
            };

            return View(viewModel);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Role.Delete)]
        public JsonResult AjaxDelete(long id)
        {
            try
            {
                var entity = _uow.BOPrinterRepository.All().FirstOrDefault(x => x.Id == id);

                if (entity == null)
                {
                    throw new InvalidOperationException(Resources.CommonStrings.RegisterNotFound);
                }

                _uow.BOPrinterRepository.Delete(entity);

                _uow.SaveChanges();

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
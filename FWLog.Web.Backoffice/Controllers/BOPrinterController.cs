using AutoMapper;
using ExtensionMethods.List;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.EnumsAndConsts.LOVs;
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
                        _uow.BOPrinterTypeRepository.Todos().Select(x => new SelectListItem
                        {
                            Value = x.Id.ToString(),
                            Text = x.Name,
                        }).ToList(), "Value", "Text");
                }

                return printertypes;
            }
        }
        private SelectList printertypes;

        private SelectList _Empresas
        {
            get
            {
                if (empresas == null)
                {
                    empresas = new SelectList(Empresas, "IdEmpresa", "Nome");
                }

                return empresas;
            }
        }
        private SelectList empresas;

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
            ViewBag.PrinterTypes = PrinterTypes;
            ViewBag.Empresas = _Empresas;
        }

        public BOPrinterController(UnitOfWork uow, BOLogSystemService boLogSystemService)
        {
            _uow = uow;
            _boLogSystemService = boLogSystemService;
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Role.List)]
        public ActionResult Index()
        {
            setViewBags();
            ViewBag.Status = Status;

            return View(new BOPrinterListViewModel());
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Role.List)]
        public ActionResult PageData(DataTableFilter<BOPrinterFilterViewModel> model)
        {
            IQueryable<Printer> all = _uow.BOPrinterRepository.All(IdEmpresasPorUsuario);

            IQueryable<Printer> query = all.WhereIf(!string.IsNullOrEmpty(model.CustomFilter.Name), x => x.Name.Contains(model.CustomFilter.Name));
            query = query.WhereIf(model.CustomFilter.IdEmpresa.HasValue, x => x.CompanyId == model.CustomFilter.IdEmpresa);
            query = query.WhereIf(model.CustomFilter.PrinterTypeId.HasValue, x => x.PrinterTypeId == model.CustomFilter.PrinterTypeId);
            query = query.WhereIf(model.CustomFilter.Status.HasValue, x => x.Ativa == model.CustomFilter.Status);

            int totalRecords = all.Count();
            int recordsFiltered = query.Count();

            var list = query.Select(x => new BOPrinterListItemViewModel
            {
                Id = x.Id,
                Empresa = x.Empresa.NomeFantasia,
                Name = x.Name,
                PrinterType = x.PrinterType.Name,
                Status = x.Ativa ? "Ativa" : "Inativa"
            });

            IList<BOPrinterListItemViewModel> result = list.PaginationResult(model);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = result
            });
        }

        [HttpGet]
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
                var entity = new Printer() { Name = model.Name, IP = model.IP, CompanyId = model.IdEmpresa, PrinterTypeId = model.PrinterTypeId, Ativa = model.Ativa };

                _uow.BOPrinterRepository.Add(entity);

                _uow.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Notify.Success(Resources.CommonStrings.RegisterCreatedSuccessMessage);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Role.List)]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var entity = _uow.BOPrinterRepository.All(IdEmpresasPorUsuario).FirstOrDefault(x => x.Id == id);

            if (entity == null)
            {
                throw new HttpException(404, "Not found");
            }

            var model = new BOPrinterDetailsViewModel()
            {
                Name = entity.Name,
                Empresa = entity.Empresa.RazaoSocial,
                PrinterType = entity.PrinterType.Name,
                IP = entity.IP,
                Ativa = entity.Ativa
            };

            return View(model);
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Role.Edit)]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var entity = _uow.BOPrinterRepository.All(IdEmpresasPorUsuario).FirstOrDefault(x => x.Id == id);

            if (entity == null)
            {
                throw new HttpException(404, "Not found");
            }

            var model = new BOPrinterCreateViewModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                IdEmpresa = entity.CompanyId,
                IP = entity.IP,
                PrinterTypeId = entity.PrinterTypeId,
                Ativa = entity.Ativa,
                RazaoEmpresa = entity.Empresa.RazaoSocial
            };

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
                var entity = _uow.BOPrinterRepository.All(IdEmpresasPorUsuario).FirstOrDefault(x => x.Id == model.Id);

                if (entity == null)
                {
                    throw new HttpException(404, "Not found");
                }

                entity.Name = model.Name;
                entity.PrinterTypeId = model.PrinterTypeId;
                entity.CompanyId = model.IdEmpresa;
                entity.IP = model.IP;
                entity.Ativa = model.Ativa;

                _uow.BOPrinterRepository.Update(entity);

                _uow.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }

            Notify.Success(Resources.CommonStrings.RegisterEditedSuccessMessage);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Selecionar(string tipo, string acao)
        {
            int idTipoImpressora = tipo.Equals("zebra", StringComparison.OrdinalIgnoreCase) ? 1 : 2;

            List<Printer> impressoras = _uow.BOPrinterRepository.All(IdEmpresasPorUsuario).Where(
                w => w.CompanyId == IdEmpresa && 
                w.Ativa == true && 
                w.PrinterTypeId == idTipoImpressora).ToList();

            var listaImpressoras = new List<BOPrinterSelecionarImpressoraViewModel>();

            foreach (Printer impressora in impressoras)
            {
                listaImpressoras.Add(new BOPrinterSelecionarImpressoraViewModel
                {
                    Id = impressora.Id,
                    Nome = impressora.Name
                });
            }

            return View(new BOPrinterSelecionarViewModel
            {
                Acao = acao,
                Impressoras = listaImpressoras
            });
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Role.Delete)]
        public JsonResult AjaxDelete(long id)
        {
            try
            {
                var entity = _uow.BOPrinterRepository.All(IdEmpresasPorUsuario).FirstOrDefault(x => x.Id == id);

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
using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
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
            ViewBag.Status = new SelectList(new List<SelectListItem>
                        {
                            new SelectListItem { Text = "Ativo", Value = "true"},
                            new SelectListItem { Text = "Inativo", Value = "false"}
                        }, "Value", "Text");

            return View(new BOPrinterListViewModel());
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Role.List)]
        public ActionResult PageData(DataTableFilter<BOPrinterFilterViewModel> model)
        {
            var filtro = Mapper.Map<DataTableFilter<ImpressoraListaFiltro>>(model);
            filtro.CustomFilter.IdEmpresa = IdEmpresa;

            List<ImpressoraListaLinhaTabela> resultado = _uow.BOPrinterRepository.BuscarLista(filtro, out int registrosFiltrados, out int totalRegistros);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = Mapper.Map<List<BOPrinterListItemViewModel>>(resultado)
            });
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Role.Create)]
        public ActionResult Create()
        {
            setViewBags();
            var viewModel = new BOPrinterCreateViewModel
            {
                Ativa = true
            };

            return View(viewModel);
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
                var entity = new Printer() { Name = model.Name, IP = model.IP, CompanyId = IdEmpresa, PrinterTypeId = model.PrinterTypeId, Ativa = model.Ativa };

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

            var entity = _uow.BOPrinterRepository.Tabela(IdEmpresasPorUsuario).FirstOrDefault(x => x.Id == id);

            if (entity == null)
            {
                throw new HttpException(404, "Not found");
            }

            var model = new BOPrinterDetailsViewModel()
            {
                Name = entity.Name,
                Empresa = entity.Empresa.NomeFantasia,
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

            var entity = _uow.BOPrinterRepository.Tabela(IdEmpresasPorUsuario).FirstOrDefault(x => x.Id == id);

            if (entity == null)
            {
                throw new HttpException(404, "Not found");
            }

            var model = new BOPrinterCreateViewModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                IP = entity.IP,
                PrinterTypeId = entity.PrinterTypeId,
                Ativa = entity.Ativa
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
                var entity = _uow.BOPrinterRepository.Tabela(IdEmpresasPorUsuario).FirstOrDefault(x => x.Id == model.Id);

                if (entity == null)
                {
                    throw new HttpException(404, "Not found");
                }

                entity.Name = model.Name;
                entity.PrinterTypeId = model.PrinterTypeId;
                entity.CompanyId = IdEmpresa;
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
        public ActionResult Selecionar(int idImpressaoItem, string acao, string id)
        {
            ImpressaoItem impressaoItem = _uow.ImpressaoItemRepository.Obter(idImpressaoItem);
            List<Printer> impressoras = _uow.BOPrinterRepository.ObterPorPerfil(IdPerfilImpressora, impressaoItem.IdImpressaoItem);

            var listaImpressoras = new SelectList(
                  impressoras.Select(x => new SelectListItem
                  {
                      Value = x.Id.ToString(),
                      Text = x.Name,
                  }), "Value", "Text");

            return View(new BOPrinterSelecionarViewModel
            {
                ImpressaoItemDescricao = impressaoItem.Descricao,
                Acao = acao,
                Id = id,
                Impressoras = listaImpressoras
            });
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Role.Delete)]
        public JsonResult AjaxDelete(long id)
        {
            try
            {
                var entity = _uow.BOPrinterRepository.Tabela(IdEmpresasPorUsuario).FirstOrDefault(x => x.Id == id);

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
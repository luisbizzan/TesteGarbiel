using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.MotivoLaudoCtx;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Web;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class MotivoLaudoController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly MotivoLaudoService _motivoLaudoService;

        public MotivoLaudoController(UnitOfWork unitOfWork, MotivoLaudoService motivoLaudoService)
        {
            _unitOfWork = unitOfWork;
            _motivoLaudoService = motivoLaudoService;
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.MotivoLaudo.Listar)]
        public ActionResult MotivoLaudo()
        {
            SetViewBags();

            return View(new MotivoLaudoListViewModel());
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.MotivoLaudo.Listar)]
        public ActionResult PageData(DataTableFilter<MotivoLaudoFiltro> model)
        {
            IEnumerable<MotivoLaudoLinhaTabela> result = _unitOfWork.MotivoLaudoRepository.ObterDadosParaDataTable(model, out int recordsFiltered, out int totalRecords);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = Mapper.Map<IEnumerable<MotivoLaudoListItemViewModel>>(result)
            });
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.MotivoLaudo.Cadastrar)]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.MotivoLaudo.Cadastrar)]
        public ActionResult Create(MotivoLaudoCreateViewModel model)
        {
            Func<ViewResult> errorView = () =>
            {
                return View(model);
            };

            if (!ModelState.IsValid)
            {
                return errorView();
            }

            var motivoLaudo = new MotivoLaudo
            {
                Descricao = model.Descricao,
                Ativo = model.Ativo
            };

            try
            {
                _motivoLaudoService.Add(motivoLaudo);

                Notify.Success(Resources.CommonStrings.RegisterCreatedSuccessMessage);
                return RedirectToAction("MotivoLaudo");
            }
            catch (DbUpdateException e)
            when (e.InnerException?.InnerException is OracleException sqlEx && sqlEx.Number == 1)
            {
                Notify.Error("Já existe um motivo cadastrado com este nome.");

                return errorView();
            }
            catch (Exception e)
            {
                Notify.Error(Resources.CommonStrings.RegisterCreatedErrorMessage);

                return errorView();
            }
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.MotivoLaudo.Editar)]
        public ActionResult ExibirModalDeEdicaoMotivoLaudo(long id)
        {
            MotivoLaudo motivoLaudo = _unitOfWork.MotivoLaudoRepository.GetById(id);

            var model = Mapper.Map<MotivoLaudoCreateViewModel>(motivoLaudo);

            return PartialView("Edit", model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.MotivoLaudo.Editar)]
        public ActionResult Edit(MotivoLaudoCreateViewModel model)
        {
            Func<ViewResult> errorView = () =>
            {
                return View(model);
            };

            if (!ModelState.IsValid)
            {
                return errorView();
            }

            var motivoLaudo = new MotivoLaudo
            {
                Descricao = model.Descricao,
                Ativo = model.Ativo,
                IdMotivoLaudo = model.IdMotivoLaudo
            };

            try
            {
                _motivoLaudoService.Edit(motivoLaudo);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = Resources.CommonStrings.RegisterEditedSuccessMessage
                });

                //Notify.Success();
                //return RedirectToAction("MotivoLaudo");
            }
            catch (DbUpdateException e)
            when (e.InnerException?.InnerException is OracleException sqlEx && sqlEx.Number == 1)
            {
                Notify.Error("");

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Já existe um motivo cadastrado com este nome."
                });

                //return errorView();
            }
            catch (Exception)
            {

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = Resources.CommonStrings.RegisterEditedErrorMessage
                });

                //return errorView();
            }
        }
    }
}
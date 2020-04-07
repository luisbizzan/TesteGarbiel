using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.GeralCtx;
using Microsoft.AspNet.Identity;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class GeralController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly GeralService _geralService;

        public GeralController(UnitOfWork unitOfWork, GeralService geralService)
        {
            _unitOfWork = unitOfWork;
            _geralService = geralService;
        }

        [HttpPost]
        public ActionResult ListarHistoricos(long Id_Categoria, long Id_Ref)
        {
            var model = new GeralHistoricoViewModel
            {
                Id_Ref = Id_Ref,
                Id_Categoria = Id_Categoria,
                Lista_Historicos = _geralService.TodosHistoricosDaCategoria(Id_Categoria, Id_Ref)
                    .Select(x => new GeralHistoricoViewModel
                    {
                        Historico = x.Historico,
                        Id_Usr = x.Id_Usr,
                        Usuario = x.Usuario,
                        Dt_Cad = x.Dt_Cad
                    }).ToList(),
            };

            return PartialView("_FormHistoricos", model);
        }

        [HttpPost]
        public ActionResult GravarHistorico(GeralHistoricoViewModel model)
        {
            Func<ViewResult> errorView = () =>
            {
                return View(model);
            };

            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values.Where(x => x.Errors.Count > 0)
                    .Aggregate("", (current, s) => current + (s.Errors[0].ErrorMessage + "<br />"));
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = erros
                });
            }

            var item = new GeralHistorico
            {
                Id_Categoria = model.Id_Categoria,
                Id_Ref = model.Id_Ref,                
                Id_Usr = User.Identity.GetUserId(),
                Historico = model.Historico
            };

            try
            {
                _geralService.InserirHistorico(item);
                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = Resources.CommonStrings.RegisterCreatedSuccessMessage
                });
            }
            catch (Exception)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = Resources.CommonStrings.RegisterEditedErrorMessage
                });
            }
        }
    }
}
using AutoMapper;
using ExtensionMethods.String;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.EmpresaCtx;
using log4net;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class EmpresaController : BOBaseController
    {
        private readonly ILog _log;
        private readonly UnitOfWork _unitOfWork;
        private readonly EmpresaService _empresaService;

        public EmpresaController(ILog log,
            UnitOfWork unitOfWork,
            EmpresaService empresaService)
        {
            _unitOfWork = unitOfWork;
            _empresaService = empresaService;
            _log = log;
        }

        public ActionResult BuscarEmpresas()
        {
            ViewData["Empresas"] = new SelectList(Empresas, "IdEmpresa", "Nome");
            return PartialView("_MudarEmpresa");
        }

        public JsonResult MudarEmpresa(int id)
        {
            try
            {
                ApplicationUser applicationUser = UserManager.FindById(User.Identity.GetUserId());

                ApplicationSession applicationSession = _unitOfWork.ApplicationSessionRepository.GetById(applicationUser.IdApplicationSession.Value);

                applicationSession.IdEmpresa = id;

                _unitOfWork.ApplicationSessionRepository.Update(applicationSession);
                _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Ocorreu um erro ao trocar a empresa.",
                    Data = Url.Action("Index", "BOHome")
                }, JsonRequestBehavior.DenyGet);
            }

            return Json(new AjaxGenericResultModel
            {
                Success = true,
                Message = Resources.CommonStrings.ChangeCompanySuccess,
                Data = Url.Action("Index", "BOHome")
            }, JsonRequestBehavior.DenyGet);
        }

        public long BuscarIdEmpresa()
        {
            return IdEmpresa;
        }

        [HttpGet]
        [ApplicationAuthorize]
        public ActionResult SearchModal(string campoSelecionado)
        {
            var model = new EmpresaSearchModalViewModel(campoSelecionado);
            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize]
        public ActionResult SearchModalPageData(DataTableFilter<EmpresaSearchModalFilterViewModel> model)
        {
            List<EmpresaSearchModalItemViewModel> empresaSearchModalItemViewModel = new List<EmpresaSearchModalItemViewModel>();
            int totalRecords = 0;
            int totalRecordsFiltered = 0;

            var query = _unitOfWork.EmpresaConfigRepository.Todos();

            totalRecords = query.Count();

            if (!string.IsNullOrEmpty(model.CustomFilter.NomeFantasia))
            {
                query = query.Where(x => x.Empresa.NomeFantasia.Contains(model.CustomFilter.NomeFantasia));
            }

            if (!string.IsNullOrEmpty(model.CustomFilter.Sigla))
            {
                query = query.Where(x => x.Empresa.Sigla == model.CustomFilter.Sigla);
            }

            if (!string.IsNullOrEmpty(model.CustomFilter.CNPJ))
            {
                query = query.Where(x => x.Empresa.CNPJ.Contains(model.CustomFilter.CNPJ.Replace(".", "").Replace("/", "").Replace("-", "")));
            }

            if (!string.IsNullOrEmpty(model.CustomFilter.CampoSelecionado) && model.CustomFilter.CampoSelecionado.Contains("Matriz"))
            {
                query = query.Where(x => x.IdEmpresaTipo != EmpresaTipoEnum.Filial);
            }

            foreach (var item in query)
            {
                empresaSearchModalItemViewModel.Add(new EmpresaSearchModalItemViewModel()
                {
                    IdEmpresa = item.IdEmpresa,
                    NomeFantasia = item.Empresa.NomeFantasia,
                    Sigla = item.Empresa.Sigla,
                    CNPJ = item.Empresa.CNPJ.MascaraCNPJ()
                });
            }

            totalRecordsFiltered = empresaSearchModalItemViewModel.Count();

            var result = empresaSearchModalItemViewModel
                .OrderBy(model.OrderByColumn, model.OrderByDirection)
                .Skip(model.Start)
                .Take(model.Length);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = totalRecordsFiltered,
                Data = result
            });
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Empresa.EditarConfiguracao)]
        public ActionResult Editar()
        {
            EmpresaConfig empresaConfig = _unitOfWork.EmpresaConfigRepository.ConsultarPorIdEmpresa(IdEmpresa);
            var model = Mapper.Map<EmpresaConfigEditarViewModel>(empresaConfig);

            model.TiposConferencia = new SelectList(_unitOfWork.TipoConferenciaRepository.RetornarTodos().Select(x => new SelectListItem
            {
                Value = x.IdTipoConferencia.GetHashCode().ToString(),
                Text = x.Descricao,
            }), "Value", "Text");

            model.TiposEmpresa = new SelectList(_unitOfWork.EmpresaTipoRepository.RetornarTodos().Select(x => new SelectListItem
            {
                Value = x.IdEmpresaTipo.GetHashCode().ToString(),
                Text = x.Descricao,
            }), "Value", "Text");

            model.DiaColeta = new SelectList(_unitOfWork.DiasDaSemanaRepository.RetornarTodos()
                .Where(x => x.IdDiasDaSemana != DiasDaSemanaEnum.Domingo && x.IdDiasDaSemana != DiasDaSemanaEnum.Sabado)
                .Select(x => new SelectListItem
                {
                    Value = x.IdDiasDaSemana.GetHashCode().ToString(),
                    Text = x.Descricao,
                }), "Value", "Text");

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Empresa.EditarConfiguracao)]
        public ActionResult Editar(EmpresaConfigEditarViewModel model)
        {
            var empresaConfigGarantia = _unitOfWork.EmpresaConfigRepository.ConsultarPorIdEmpresa(model.IdEmpresaGarantia);

            if (empresaConfigGarantia == null)
            {
                ModelState.AddModelError(nameof(model.IdEmpresaGarantia), "Empresa Garantia não encontrada.");
            }

            if (model.IdEmpresa == model.IdEmpresaGarantia && !model.EmpresaFazGarantia)
            {
                ModelState.AddModelError(nameof(model.EmpresaFazGarantia), "A empresa editada não pode ser selecionada para Garantia, se não estiver marcada para fazer garantia.");
            }

            if (empresaConfigGarantia?.EmpresaFazGarantia == false)
            {
                ModelState.AddModelError(nameof(model.IdEmpresaGarantia), string.Format("A Empresa '{0}' não pode ser selecionada para Garantia, se não estiver marcada para fazer garantia.", model.NomeFantasiaEmpresaGarantia));
            }

            if (!ModelState.IsValid)
            {
                model.TiposConferencia = new SelectList(_unitOfWork.TipoConferenciaRepository.RetornarTodos().Select(x => new SelectListItem
                {
                    Value = x.IdTipoConferencia.GetHashCode().ToString(),
                    Text = x.Descricao,
                }), "Value", "Text");

                model.TiposEmpresa = new SelectList(_unitOfWork.EmpresaTipoRepository.RetornarTodos().Select(x => new SelectListItem
                {
                    Value = x.IdEmpresaTipo.GetHashCode().ToString(),
                    Text = x.Descricao,
                }), "Value", "Text");

                model.DiaColeta = new SelectList(_unitOfWork.DiasDaSemanaRepository.RetornarTodos()
                .Where(x => x.IdDiasDaSemana != DiasDaSemanaEnum.Domingo && x.IdDiasDaSemana != DiasDaSemanaEnum.Sabado)
                .Select(x => new SelectListItem
                {
                    Value = x.IdDiasDaSemana.GetHashCode().ToString(),
                    Text = x.Descricao,
                }), "Value", "Text");

                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(model.CNPJConferenciaAutomatica))
            {
                model.CNPJConferenciaAutomatica = Regex.Replace(model.CNPJConferenciaAutomatica, @"[^\d]", string.Empty);
            }

            EmpresaConfig empresaConfig = Mapper.Map<EmpresaConfig>(model);

            _empresaService.Editar(empresaConfig);

            Notify.Success(Resources.CommonStrings.RegisterEditedSuccessMessage);
            return RedirectToAction("Editar");
        }
    }
}
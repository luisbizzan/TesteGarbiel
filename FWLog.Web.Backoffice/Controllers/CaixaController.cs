using AutoMapper;
using DartDigital.Library.Exceptions;
using FWLog.AspNet.Identity;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CaixaCtx;
using FWLog.Web.Backoffice.Models.CommonCtx;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class CaixaController : BOBaseController
    {
        private readonly CaixaService _caixaService;

        public CaixaController(CaixaService caixaService)
        {
            _caixaService = caixaService;
        }

        private SelectList BuscarCaixaTipoSelectList()
        {
            return new SelectList(_caixaService.BuscarTodosCaixaTipo().OrderBy(o => o.IdCaixaTipo).Select(x => new SelectListItem
            {
                Value = x.IdCaixaTipo.GetHashCode().ToString(),
                Text = x.Descricao,
            }), "Value", "Text");
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Caixa.Listar)]
        public ActionResult Index()
        {
            var model = new CaixaListaViewModel();

            model.ListaStatus = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Ativo", Value = "true"},
                new SelectListItem { Text = "Inativo", Value = "false"}
            }, "Value", "Text");


            model.ListaCaixaTipo = BuscarCaixaTipoSelectList();

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Caixa.Listar)]
        public ActionResult DadosLista(DataTableFilter<CaixaListaFiltro> model)
        {
            model.CustomFilter.IdEmpresa = IdEmpresa;

            List<CaixaListaTabela> result = _caixaService.BuscarLista(model, out int registrosFiltrados, out int totalRegistros);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = Mapper.Map<IEnumerable<CaixaListaTabela>>(result)
            });
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Caixa.Cadastrar)]
        public ActionResult Cadastrar()
        {
            return View(new CaixaCadastroViewModel
            {
                ListaCaixaTipo = BuscarCaixaTipoSelectList(),
                Ativo = true
            });
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Caixa.Cadastrar)]
        public ActionResult Cadastrar(CaixaCadastroViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.ListaCaixaTipo = BuscarCaixaTipoSelectList();

                return View(viewModel);
            }

            try
            {
                var caixa = Mapper.Map<Caixa>(viewModel);

                caixa.IdEmpresa = IdEmpresa;

                _caixaService.Cadastrar(caixa, IdEmpresa);

                Notify.Success("Caixa cadastrada com sucesso.");
            }
            catch (BusinessException businessException)
            {
                ModelState.AddModelError(string.Empty, businessException.Message);

                viewModel.ListaCaixaTipo = BuscarCaixaTipoSelectList();

                return View(viewModel);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Caixa.Editar)]
        public ActionResult Editar(long id)
        {
            var caixa = _caixaService.GetCaixaById(id);

            var viewModel = Mapper.Map<CaixaEdicaoViewModel>(caixa);

            viewModel.ListaCaixaTipo = BuscarCaixaTipoSelectList();

            return View(viewModel);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Caixa.Editar)]
        public ActionResult Editar(CaixaEdicaoViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.ListaCaixaTipo = BuscarCaixaTipoSelectList();

                return View(viewModel);
            }

            try
            {
                var caixa = Mapper.Map<Caixa>(viewModel);

                _caixaService.Editar(caixa, IdEmpresa);

                Notify.Success("Caixa editada com sucesso.");

                return RedirectToAction("Index");
            }
            catch (BusinessException businessException)
            {
                ModelState.AddModelError(string.Empty, businessException.Message);

                viewModel.ListaCaixaTipo = BuscarCaixaTipoSelectList();

                return View(viewModel);
            }
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Caixa.Visualizar)]
        public ActionResult Detalhes(long id)
        {
            var caixa = _caixaService.GetCaixaById(id);

            var viewModel = Mapper.Map<CaixaDetalhesViewModel>(caixa);

            return View(viewModel);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Caixa.Excluir)]
        public JsonResult ExcluirAjax(int id)
        {
            try
            {
                _caixaService.Excluir(id);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = Resources.CommonStrings.RegisterDeletedSuccessMessage
                }, JsonRequestBehavior.DenyGet);
            }
            catch (BusinessException exception)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = exception.Message
                }, JsonRequestBehavior.DenyGet);
            }
        }

        [ApplicationAuthorize]
        public ActionResult SearchModal(long? id = null)
        {
            var model = new CaixaSearchModalViewModel();

            model.ListaStatus = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Ativo", Value = "true"},
                new SelectListItem { Text = "Inativo", Value = "false"}
            }, "Value", "Text");


            model.ListaCaixaTipo = BuscarCaixaTipoSelectList();

            return View(model);
        }

        [ApplicationAuthorize]
        public ActionResult SearchModalPageData(DataTableFilter<CaixaListaFiltro> filtro)
        {
            filtro.CustomFilter.IdEmpresa = IdEmpresa;

            var result = _caixaService.BuscarLista(filtro, out int registrosFiltrados, out int totalRegistros);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = filtro.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = Mapper.Map<IEnumerable<CaixaListaTabela>>(result)
            });
        }
    }
}
using AutoMapper;
using DartDigital.Library.Exceptions;
using FWLog.AspNet.Identity;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CaixaRecusaCtx;
using FWLog.Web.Backoffice.Models.CommonCtx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class CaixaRecusaController : BOBaseController
    {
        private readonly CaixaRecusaService _caixaRecusaService;

        public CaixaRecusaController(CaixaRecusaService caixaRecusaService)
        {
            _caixaRecusaService = caixaRecusaService;
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.CaixaRecusa.Listar)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.CaixaRecusa.Listar)]
        public ActionResult DadosLista(DataTableFilter<CaixaRecusaListaFiltro> model)
        {
            model.CustomFilter.IdEmpresa = IdEmpresa;

            var result = _caixaRecusaService.BuscarLista(model, out int registrosFiltrados, out int totalRegistros);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = Mapper.Map<IEnumerable<CaixaRecusaListaTabela>>(result)
            });
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.CaixaRecusa.Cadastrar)]
        public ActionResult Cadastrar()
        {
            var model = new CaixaRecusaCadastroViewModel()
            {
                IdEmpresa = IdEmpresa
            };

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.CaixaRecusa.Cadastrar)]
        public ActionResult VerificarCaixaCadastrada(long idCaixa)
        {
            try
            {
                var result = _caixaRecusaService.ExisteCaixaRecusa(IdEmpresa, idCaixa);

                if (result)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = true,
                        Message = ""
                    });
                }
                else
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "",
                    });
                }
            }
            catch 
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Ocorreu um erro na verificação da Caixa."
                }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Caixa.Cadastrar)]
        public JsonResult Cadastrar(List<CaixaRecusaCadastroViewModel> listaCaixaRecusa)
        {
            try
            {
                var caixaRecusa = Mapper.Map<List<CaixaRecusa>>(listaCaixaRecusa);

                _caixaRecusaService.Cadastrar(caixaRecusa, IdEmpresa);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Caixa e produtos de recusa cadastradros com sucesso."
                });
            }
            catch (BusinessException businessException)
            {
                ModelState.AddModelError(string.Empty, businessException.Message);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Erro ao salvar caixa e produtos de recusa."
                });
            }
        }

        //[HttpGet]
        //[ApplicationAuthorize(Permissions = Permissions.Caixa.Editar)]
        //public ActionResult Editar(long id)
        //{
        //    var caixa = _caixaService.GetCaixaById(id);

        //    var viewModel = Mapper.Map<CaixaEdicaoViewModel>(caixa);

        //    viewModel.ListaCaixaTipo = BuscarCaixaTipoSelectList();

        //    return View(viewModel);
        //}

        //[HttpPost]
        //[ApplicationAuthorize(Permissions = Permissions.Caixa.Editar)]
        //public ActionResult Editar(CaixaEdicaoViewModel viewModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        viewModel.ListaCaixaTipo = BuscarCaixaTipoSelectList();

        //        return View(viewModel);
        //    }

        //    try
        //    {
        //        var caixa = Mapper.Map<Caixa>(viewModel);

        //        _caixaService.Editar(caixa, IdEmpresa);

        //        Notify.Success("Caixa editada com sucesso.");

        //        return RedirectToAction("Index");
        //    }
        //    catch (BusinessException businessException)
        //    {
        //        ModelState.AddModelError(string.Empty, businessException.Message);

        //        viewModel.ListaCaixaTipo = BuscarCaixaTipoSelectList();

        //        return View(viewModel);
        //    }
        //}

        //[HttpGet]
        //[ApplicationAuthorize(Permissions = Permissions.Caixa.Visualizar)]
        //public ActionResult Detalhes(long id)
        //{
        //    var caixa = _caixaService.GetCaixaById(id);

        //    var viewModel = Mapper.Map<CaixaDetalhesViewModel>(caixa);

        //    return View(viewModel);
        //}
    }
}
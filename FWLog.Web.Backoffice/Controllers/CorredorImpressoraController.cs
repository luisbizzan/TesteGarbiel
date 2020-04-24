using AutoMapper;
using DartDigital.Library.Exceptions;
using FWLog.AspNet.Identity;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.CorredorImpressoraCtx;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class CorredorImpressoraController : BOBaseController
    {
        private readonly CorredorImpressoraService _service;

        public CorredorImpressoraController(CorredorImpressoraService service)
        {
            _service = service;
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Separacao.ListarCorredorImpressora)]
        public ActionResult Index()
        {
            var model = new CorredorImpressoraListaViewModel
            {
                Filtros = new CorredorImpressoraListaFilterViewModel()
                {
                    ListaImpressora = new SelectList(
                    _service.BuscarImpressoraPorEmpresa(IdEmpresa).Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name,
                    }), "Value", "Text"
                    ),
                    ListaStatus = new SelectList(new List<SelectListItem>
                    {
                        new SelectListItem { Text = "Ativo", Value = "true"},
                        new SelectListItem { Text = "Inativo", Value = "false"}
                    }, "Value", "Text")
                }
            };

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Separacao.ListarCorredorImpressora)]
        public ActionResult DadosLista(DataTableFilter<CorredorImpressoraListaFiltro> model)
        {
            model.CustomFilter.IdEmpresa = IdEmpresa;

            var result = _service.BuscarLista(model, out int registrosFiltrados, out int totalRegistros);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = Mapper.Map<IEnumerable<CorredorImpressoraListaTabela>>(result)
            });
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Separacao.CadastrarCorredorImpressora)]
        public ActionResult Cadastrar()
        {
            return View(new CorredorImpressoraCadastroViewModel
            {
                ListaImpressora = new SelectList(
                    _service.BuscarImpressoraPorEmpresa(IdEmpresa).Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name,
                    }), "Value", "Text"
                    ),
                Ativo = true
            });
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Separacao.CadastrarCorredorImpressora)]
        public ActionResult Cadastrar(CorredorImpressoraCadastroViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.ListaImpressora = new SelectList(
                    _service.BuscarImpressoraPorEmpresa(IdEmpresa).Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name,
                    }), "Value", "Text"
                    );

                return View(viewModel);
            }

            try
            {
                var corredorImpressora = Mapper.Map<GrupoCorredorArmazenagem>(viewModel);

                corredorImpressora.IdEmpresa = IdEmpresa;

                _service.Cadastrar(corredorImpressora);

                Notify.Success("Corredor x impressora cadastrado com sucesso.");
            }
            catch (BusinessException businessException)
            {
                ModelState.AddModelError(string.Empty, businessException.Message);

                viewModel.ListaImpressora = new SelectList(
                    _service.BuscarImpressoraPorEmpresa(IdEmpresa).Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name,
                    }), "Value", "Text"
                    );

                return View(viewModel);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Separacao.VisualizarCorredorImpressora)]
        public ActionResult Detalhes(long id)
        {
            var corredorImpressora = _service.GetCorredorImpressoraById(id);

            var viewModel = Mapper.Map<CorredorImpressoraDetalhesViewModel>(corredorImpressora);

            return View(viewModel);
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Separacao.EditarCorredorImpressora)]
        public ActionResult Editar(long id)
        {
            var corredorImpressora = _service.GetCorredorImpressoraById(id);

            var viewModel = Mapper.Map<CorredorImpressoraEdicaoViewModel>(corredorImpressora);

            viewModel.ListaImpressora = new SelectList(
                    _service.BuscarImpressoraPorEmpresa(IdEmpresa).Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name,
                    }), "Value", "Text"
                    );

            return View(viewModel);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Separacao.EditarCorredorImpressora)]
        public ActionResult Editar(CorredorImpressoraEdicaoViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.ListaImpressora = new SelectList(
                    _service.BuscarImpressoraPorEmpresa(IdEmpresa).Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name,
                    }), "Value", "Text"
                    );

                return View(viewModel);
            }

            try
            {
                var corredorImpressora = Mapper.Map<GrupoCorredorArmazenagem>(viewModel);

                _service.Editar(corredorImpressora, IdEmpresa);

                Notify.Success("Corredor x impressora editado com sucesso.");

                return RedirectToAction("Index");
            }
            catch (BusinessException businessException)
            {
                ModelState.AddModelError(string.Empty, businessException.Message);

                viewModel.ListaImpressora = new SelectList(
                    _service.BuscarImpressoraPorEmpresa(IdEmpresa).Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name,
                    }), "Value", "Text"
                    );

                return View(viewModel);
            }
        }
    }
}
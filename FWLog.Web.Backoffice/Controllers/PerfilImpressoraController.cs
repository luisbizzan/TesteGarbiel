using AutoMapper;
using FWLog.Data;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.PerfilImpressoraCtx;
using FWLog.Web.Backoffice.Models.CommonCtx;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;
using FWLog.AspNet.Identity;
using System.Threading.Tasks;
using System.Linq;

namespace FWLog.Web.Backoffice.Controllers
{
    public class PerfilImpressoraController : BOBaseController
    {
        UnitOfWork _uow;
        PerfilImpressoraService _perfilImpressoraService;

        public PerfilImpressoraController(UnitOfWork uow, PerfilImpressoraService perfilImpressoraService)
        {
            _uow = uow;
            _perfilImpressoraService = perfilImpressoraService;
        }

        [ApplicationAuthorize(Permissions = Permissions.PerfilImpressora.Listar)]
        public ActionResult Index()
        {
            var viewModel = new PerfilImpressoraListViewModel
            {
                Status = new SelectList(new List<SelectListItem>
                        {
                            new SelectListItem { Text = "Ativo", Value = "1"},
                            new SelectListItem { Text = "Inativo", Value = "0"}
                        }, "Value", "Text")
            };

            return View(viewModel);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.PerfilImpressora.Listar)]
        public ActionResult DadosLista(DataTableFilter<PerfilImpressoraFilterViewModel> model)
        {
            int recordsFiltered, totalRecords;
            var filter = Mapper.Map<DataTableFilter<PerfilImpressoraFilter>>(model);
            filter.CustomFilter.IdEmpresa = IdEmpresa;
            IEnumerable<PerfilImpressoraTableRow> result = _uow.PerfilImpressoraRepository.BuscarLista(filter, out recordsFiltered, out totalRecords);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = Mapper.Map<IEnumerable<PerfilImpressoraListItemViewModel>>(result)
            });
        }

        [ApplicationAuthorize(Permissions = Permissions.PerfilImpressora.Criar)]
        public ActionResult Cadastrar()
        {
            CarregarDadosImpressaoItem();

            var model = new PerfilImpressoraCreateViewModel()
            {
                Ativo = true
            };

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.PerfilImpressora.Criar)]
        public ActionResult Cadastrar(PerfilImpressoraCreateViewModel model)
        {
            //TODO validações
            if (!ModelState.IsValid)
            {
                CarregarDadosImpressaoItem();

                return View(model);
            }

            PerfilImpressora perfilImpressora = Mapper.Map<PerfilImpressora>(model);
            perfilImpressora.IdEmpresa = IdEmpresa;

            _perfilImpressoraService.Add(perfilImpressora);

            Notify.Success(Resources.CommonStrings.RegisterCreatedSuccessMessage);
            return RedirectToAction("Index");
        }

        [ApplicationAuthorize(Permissions = Permissions.PerfilImpressora.Editar)]
        public ActionResult Editar(int id)
        {
            CarregarDadosImpressaoItem();

            PerfilImpressora perfilImpressora = _uow.PerfilImpressoraRepository.GetById(id);

            if (perfilImpressora == null)
            {
                throw new HttpException(404, "Not found");
            }

            var model = Mapper.Map<PerfilImpressoraCreateViewModel>(perfilImpressora);

            List<Printer> impressoras = _uow.BOPrinterRepository.ObterPorEmpresa(IdEmpresa);

            foreach (var modelTipos in model.TiposImpressao)
            {
                foreach (var impressora in impressoras)
                {
                    if (!modelTipos.Impressoras.Any(a => a.IdImpressora == impressora.Id))
                    {
                        var impressoraView = new ImpressoraViewModel()
                        {
                            IdImpressora = impressora.Id,
                            Nome = impressora.Name,
                            Selecionado = false
                        };

                        modelTipos.Impressoras.Add(impressoraView);
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.PerfilImpressora.Editar)]
        public ActionResult Editar(PerfilImpressoraCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                CarregarDadosImpressaoItem();

                return View(model);
            }

            PerfilImpressora perfilImpressora = Mapper.Map<PerfilImpressora>(model);

            _perfilImpressoraService.Edit(perfilImpressora);

            Notify.Success(Resources.CommonStrings.RegisterEditedSuccessMessage);
            return RedirectToAction("Index");
        }

        [ApplicationAuthorize(Permissions = Permissions.PerfilImpressora.Listar)]
        public ActionResult Detalhes(int id)
        {
            CarregarDadosImpressaoItem();

            PerfilImpressora perfilImpressora = _uow.PerfilImpressoraRepository.GetById(id);

            if (perfilImpressora == null)
            {
                throw new HttpException(404, "Not found");
            }

            var model = Mapper.Map<PerfilImpressoraDetailsViewModel>(perfilImpressora);

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.PerfilImpressora.Excluir)]
        public JsonResult AjaxDelete(int id)
        {
            try
            {
                _perfilImpressoraService.Delete(_uow.PerfilImpressoraRepository.GetById(id));

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = Resources.CommonStrings.RegisterDeletedSuccessMessage
                }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception e)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = Resources.CommonStrings.RegisterHasRelationshipsErrorMessage
                }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.BOAccount.Create)]
        public async Task<ActionResult> AdicionarTipoImpressao(int id)
        {
            List<Printer> impressoras = _uow.BOPrinterRepository.ObterPorEmpresa(IdEmpresa);

            List<ImpressaoItem> ListImpressaoItem = _uow.ImpressaoItemRepository.Todos();

            var tiposImpressao = new TipoImpressaoViewModel
            {
                IdImpressaoItem = id,
                Descricao = ListImpressaoItem.First(f => (int)f.IdImpressaoItem == id).Descricao
            };

            foreach (var impressora in impressoras)
            {
                var impressoraViewModel = new ImpressoraViewModel
                {
                    Selecionado = false,
                    Nome = impressora.Name,
                    IdImpressora = impressora.Id
                };

                tiposImpressao.Impressoras.Add(impressoraViewModel);
            }

            var list = new List<TipoImpressaoViewModel>
            {
                tiposImpressao
            };

            return PartialView("_ListaImpressoras", list);
        }

        private void CarregarDadosImpressaoItem()
        {
            ViewBag.ImpressaoItens = new SelectList(
                   _uow.ImpressaoItemRepository.Todos().Select(x => new SelectListItem
                   {
                       Value = x.IdImpressaoItem.GetHashCode().ToString(),
                       Text = x.Descricao,
                   }), "Value", "Text");

        }
    }
}

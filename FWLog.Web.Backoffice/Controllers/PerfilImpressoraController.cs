using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.PerfilImpressoraCtx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class PerfilImpressoraController : BOBaseController
    {
        private readonly UnitOfWork _uow;
        private readonly PerfilImpressoraService _perfilImpressoraService;

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
            Func<string, ViewResult> errorView = (error) =>
            {
                CarregarDadosImpressaoItem();

                if (error != null)
                {
                    Notify.Error(Resources.CommonStrings.RequestUnexpectedErrorMessage);
                }

                return View(model);
            };

            if (!ModelState.IsValid)
            {
                return errorView(null);
            }

            if (model.TiposImpressao.NullOrEmpty())
            {
                ModelState.AddModelError(nameof(model.TiposImpressao), "Selecione pelo menos um tipo de impressão para o perfil.");
                return errorView(null);
            }

            var teste = model.TiposImpressao.Where(w => w.Impressoras.Any(a => a.Selecionado)).Any();
            if (!model.TiposImpressao.Where(w => w.Impressoras.Any(a => a.Selecionado)).Any())
            {
                ModelState.AddModelError(nameof(model.TiposImpressao), "Escolha pelo menos uma impressora para o perfil");
                return errorView(null);
            }

            var teste2 = model.TiposImpressao.Where(w => w.Impressoras.Any(a => a.Selecionado)).Any();
            if (model.TiposImpressao.Where(w => !w.Impressoras.Any(a => a.Selecionado)).Any())
            {
                ModelState.AddModelError(nameof(model.TiposImpressao), "Todos os tipos de impressão selecionados devem ter pelos menos um empresa escolhida.");
                return errorView(null);
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

            PerfilImpressora perfilImpressora = _uow.PerfilImpressoraRepository.ObterPorIdImpressorasAtivas(id);

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

                modelTipos.Impressoras = modelTipos.Impressoras.Where(x=> x.IdImpressora != 0).OrderBy(o => o.Nome).ToList();
            }

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.PerfilImpressora.Editar)]
        public ActionResult Editar(PerfilImpressoraCreateViewModel model)
        {
            Func<string, ViewResult> errorView = (error) =>
            {
                CarregarDadosImpressaoItem();

                if (error != null)
                {
                    Notify.Error(Resources.CommonStrings.RequestUnexpectedErrorMessage);
                }

                return View(model);
            };

            if (!ModelState.IsValid)
            {
                return errorView(null);
            }

            if (model.TiposImpressao.NullOrEmpty())
            {
                ModelState.AddModelError(nameof(model.TiposImpressao), "Selecione pelo menos um tipo de impressão para o perfil.");
                return errorView(null);
            }

            if (!model.TiposImpressao.Where(w => w.Impressoras.Any(a => a.Selecionado)).Any())
            {
                ModelState.AddModelError(nameof(model.TiposImpressao), "Escolha uma impressora para um tipo de impressao para o perfil");
                return errorView(null);
            }

            if (model.TiposImpressao.Where(w => !w.Impressoras.Any(a => a.Selecionado)).Any())
            {
                ModelState.AddModelError(nameof(model.TiposImpressao), "Todos os tipos de impressão selecionados devem ter pelos menos um empresa escolhida.");
                return errorView(null);
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

            PerfilImpressora perfilImpressora = _uow.PerfilImpressoraRepository.ObterPorIdImpressorasAtivas(id);

            if (perfilImpressora == null)
            {
                throw new HttpException(404, "Not found");
            }

            var model = Mapper.Map<PerfilImpressoraCreateViewModel>(perfilImpressora);

            foreach (var modelTipos in model.TiposImpressao)
            {
                modelTipos.Impressoras = modelTipos.Impressoras.Where(x => x.IdImpressora != 0).OrderBy(o => o.Nome).ToList();
            }

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
            catch (Exception)
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

        public ActionResult BuscarPerfilImpressora()
        {
            long idPerfilImpressora = IdPerfilImpressora;

            List<SelectListItem> list = new List<SelectListItem>();

            if (idPerfilImpressora == 0)
            {
                list.Add(new SelectListItem { Value = 0.ToString(), Text = "Perfil de Impressão não Configurado", Disabled = true });
            }

            list.AddRange(_uow.PerfilImpressoraRepository.RetornarAtivas().Where(x => x.IdEmpresa == IdEmpresa).Select(x => new SelectListItem
            {
                Value = x.IdPerfilImpressora.ToString(),
                Text = x.Nome,
                Selected = x.IdPerfilImpressora == idPerfilImpressora
            }).ToList());

            ViewBag.Perfis = new SelectList(list, "Value", "Text", idPerfilImpressora, new[] { 0.ToString() });

            return PartialView("_MudarPerfilImpressora");
        }

        public void DefinePerfilImpressoraSessao(long idPerfil)
        {
            IdPerfilImpressora = idPerfil;
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

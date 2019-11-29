using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.EnderecoArmazenagemCtx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class EnderecoArmazenagemController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly EnderecoArmazenagemService _enderecoArmazenagemService;

        public EnderecoArmazenagemController(
            UnitOfWork unitOfWork,
            EnderecoArmazenagemService enderecoArmazenagemService)
        {
            _unitOfWork = unitOfWork;
            _enderecoArmazenagemService = enderecoArmazenagemService;
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.EnderecoArmazenagem.Listar)]
        public ActionResult Index()
        {
            var viewModel = new EnderecoArmazenagemListaViewModel
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
        [ApplicationAuthorize(Permissions = Permissions.EnderecoArmazenagem.Listar)]
        public ActionResult DadosLista(DataTableFilter<EnderecoArmazenagemListaFilterViewModel> model)
        {
            var filtro = Mapper.Map<DataTableFilter<EnderecoArmazenagemListaFiltro>>(model);
            filtro.CustomFilter.IdEmpresa = IdEmpresa;

            IEnumerable<EnderecoArmazenagemListaLinhaTabela> result = _unitOfWork.EnderecoArmazenagemRepository.BuscarLista(filtro, out int registrosFiltrados, out int totalRegistros);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = Mapper.Map<IEnumerable<EnderecoArmazenagemListaItemViewModel>>(result)
            });
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.EnderecoArmazenagem.Cadastrar)]
        public ActionResult Cadastrar()
        {
            var viewModel = new EnderecoArmazenagemCadastroViewModel
            {
                Ativo = true
            };

            return View(viewModel);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.EnderecoArmazenagem.Cadastrar)]
        public ActionResult Cadastrar(EnderecoArmazenagemCadastroViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var enderecoArmazenagem = Mapper.Map<EnderecoArmazenagem>(viewModel);
            enderecoArmazenagem.IdEmpresa = IdEmpresa;

            _enderecoArmazenagemService.Cadastrar(enderecoArmazenagem);

            Notify.Success("Endereço de Armazenagem cadastrado com sucesso.");
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.EnderecoArmazenagem.Excluir)]
        public JsonResult ExcluirAjax(int id)
        {
            try
            {
                _enderecoArmazenagemService.Excluir(id);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = Resources.CommonStrings.RegisterDeletedSuccessMessage
                }, JsonRequestBehavior.DenyGet);
            }
            catch
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = Resources.CommonStrings.RegisterHasRelationshipsErrorMessage
                }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.EnderecoArmazenagem.Editar)]
        public ActionResult Editar(long id)
        {
            EnderecoArmazenagem enderecoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.GetById(id);

            var viewModel = Mapper.Map<EnderecoArmazenagemEditarViewModel>(enderecoArmazenagem);

            return View(viewModel);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.EnderecoArmazenagem.Editar)]
        public ActionResult Editar(EnderecoArmazenagemEditarViewModel viewModel)
        {
            var enderecoArmazenagem = Mapper.Map<EnderecoArmazenagem>(viewModel);
            enderecoArmazenagem.IdEmpresa = IdEmpresa;

            _enderecoArmazenagemService.Editar(enderecoArmazenagem);

            Notify.Success("Endereço de Armazenagem editado com sucesso.");
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.EnderecoArmazenagem.Visualizar)]
        public ActionResult Detalhes(long id)
        {
            EnderecoArmazenagem enderecoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.GetById(id);

            var viewModel = Mapper.Map<EnderecoArmazenagemDetalhesViewModel>(enderecoArmazenagem);

            return View(viewModel);
        }
    }
}
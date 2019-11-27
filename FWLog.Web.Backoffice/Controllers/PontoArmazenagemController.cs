using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.PontoArmazenagemCtx;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class PontoArmazenagemController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly PontoArmazenagemService _pontoArmazenagemService;

        public PontoArmazenagemController (
            UnitOfWork unitOfWork,
            PontoArmazenagemService pontoArmazenagemService)
        {
            _unitOfWork = unitOfWork;
            _pontoArmazenagemService = pontoArmazenagemService;
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.PontoArmazenagem.Listar)]
        public ActionResult Index()
        {
            var viewModel = new PontoArmazenagemListaViewModel
            {
                TiposArmazenagem = new SelectList(_unitOfWork.TipoArmazenagemRepository.RetornarTodos().Select(x => new SelectListItem
                                    {
                                        Value = x.IdTipoArmazenagem.GetHashCode().ToString(),
                                        Text = x.Descricao,
                                    }), "Value", "Text"),
                TiposMovimentacao = new SelectList(_unitOfWork.TipoMovimentacaoRepository.RetornarTodos().Select(x => new SelectListItem
                                    {
                                        Value = x.IdTipoMovimentacao.GetHashCode().ToString(),
                                        Text = x.Descricao,
                                    }), "Value", "Text"),
                Status = new SelectList(new List<SelectListItem>
                        {
                            new SelectListItem { Text = "Ativo", Value = "1"},
                            new SelectListItem { Text = "Inativo", Value = "0"}
                        }, "Value", "Text")
            };

            return View(viewModel);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.PontoArmazenagem.Listar)]
        public ActionResult DadosLista(DataTableFilter<PontoArmazenagemListaFilterViewModel> model)
        {
            var filtro = Mapper.Map<DataTableFilter<PontoArmazenagemListaFiltro>>(model);
            filtro.CustomFilter.IdEmpresa = IdEmpresa;

            IEnumerable<PontoArmazenagemListaLinhaTabela> result = _unitOfWork.PontoArmazenagemRepository.BuscarLista(filtro, out int registrosFiltrados, out int totalRegistros);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = Mapper.Map<IEnumerable<PontoArmazenagemListaItemViewModel>>(result)
            });
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.PontoArmazenagem.Cadastrar)]
        public ActionResult Cadastrar()
        {
            var viewModel = new PontoArmazenagemCadastroViewModel
            {
                TiposArmazenagem = new SelectList(_unitOfWork.TipoArmazenagemRepository.RetornarTodos().Select(x => new SelectListItem
                {
                    Value = x.IdTipoArmazenagem.GetHashCode().ToString(),
                    Text = x.Descricao,
                }), "Value", "Text"),
                TiposMovimentacao = new SelectList(_unitOfWork.TipoMovimentacaoRepository.RetornarTodos().Select(x => new SelectListItem
                {
                    Value = x.IdTipoMovimentacao.GetHashCode().ToString(),
                    Text = x.Descricao,
                }), "Value", "Text"),
                Ativo = true
            };

            return View(viewModel);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.PontoArmazenagem.Cadastrar)]
        public ActionResult Cadastrar(PontoArmazenagemCadastroViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.TiposArmazenagem = new SelectList(_unitOfWork.TipoArmazenagemRepository.RetornarTodos().Select(x => new SelectListItem
                {
                    Value = x.IdTipoArmazenagem.GetHashCode().ToString(),
                    Text = x.Descricao,
                }), "Value", "Text");
                viewModel.TiposMovimentacao = new SelectList(_unitOfWork.TipoMovimentacaoRepository.RetornarTodos().Select(x => new SelectListItem
                {
                    Value = x.IdTipoMovimentacao.GetHashCode().ToString(),
                    Text = x.Descricao,
                }), "Value", "Text");

                return View(viewModel);
            }

            var pontoArmazenagem = new PontoArmazenagem
            {
                IdEmpresa = IdEmpresa,
                IdNivelArmazenagem = viewModel.IdNivelArmazenagem.Value,
                Descricao = viewModel.Descricao,
                IdTipoArmazenagem = viewModel.IdTipoArmazenagem,
                IdTipoMovimentacao = viewModel.IdTipoMovimentacao,
                LimitePesoVertical = viewModel.LimitePesoVertical,
                Ativo = viewModel.Ativo
            };

            _pontoArmazenagemService.Cadastrar(pontoArmazenagem);

            Notify.Success("Ponto de Armazenagem cadastrado com sucesso.");
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.PontoArmazenagem.Editar)]
        public ActionResult Editar(long id)
        {
            PontoArmazenagem pontoArmazenagem = _unitOfWork.PontoArmazenagemRepository.GetById(id);

            var viewModel = Mapper.Map<PontoArmazenagemEditarViewModel>(pontoArmazenagem);

            viewModel.TiposArmazenagem = new SelectList(_unitOfWork.TipoArmazenagemRepository.RetornarTodos().Select(x => new SelectListItem
            {
                Value = x.IdTipoArmazenagem.GetHashCode().ToString(),
                Text = x.Descricao,
            }), "Value", "Text");
            viewModel.TiposMovimentacao = new SelectList(_unitOfWork.TipoMovimentacaoRepository.RetornarTodos().Select(x => new SelectListItem
            {
                Value = x.IdTipoMovimentacao.GetHashCode().ToString(),
                Text = x.Descricao,
            }), "Value", "Text");

            return View(viewModel);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.PontoArmazenagem.Editar)]
        public ActionResult Editar(PontoArmazenagemEditarViewModel viewModel)
        {
            var pontoArmazenagem = Mapper.Map<PontoArmazenagem>(viewModel);
            pontoArmazenagem.IdEmpresa = IdEmpresa;

            _pontoArmazenagemService.Editar(pontoArmazenagem);

            Notify.Success("Ponto de Armazenagem editado com sucesso.");
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.PontoArmazenagem.Editar)]
        [Route("{idPontoArmazenagem: long}")]
        public ActionResult Detalhes(long idPontoArmazenagem)
        {
            PontoArmazenagem pontoArmazenagem = _unitOfWork.PontoArmazenagemRepository.GetById(idPontoArmazenagem);

            var viewModel = Mapper.Map<PontoArmazenagemEditarViewModel>(pontoArmazenagem);

            viewModel.TiposArmazenagem = new SelectList(_unitOfWork.TipoArmazenagemRepository.RetornarTodos().Select(x => new SelectListItem
            {
                Value = x.IdTipoArmazenagem.GetHashCode().ToString(),
                Text = x.Descricao,
            }), "Value", "Text");
            viewModel.TiposMovimentacao = new SelectList(_unitOfWork.TipoMovimentacaoRepository.RetornarTodos().Select(x => new SelectListItem
            {
                Value = x.IdTipoMovimentacao.GetHashCode().ToString(),
                Text = x.Descricao,
            }), "Value", "Text");

            return View(viewModel);
        }
    }
}
using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Model.Etiquetas;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.EnderecoArmazenagemCtx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class EnderecoArmazenagemController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly EnderecoArmazenagemService _enderecoArmazenagemService;
        private readonly EtiquetaService _etiquetaService;
        private readonly ApplicationLogService _applicationLogService;

        public EnderecoArmazenagemController(
            UnitOfWork unitOfWork,
            EnderecoArmazenagemService enderecoArmazenagemService,
            EtiquetaService etiquetaService,
            ApplicationLogService applicationLogService)
        {
            _unitOfWork = unitOfWork;
            _enderecoArmazenagemService = enderecoArmazenagemService;
            _etiquetaService = etiquetaService;
            _applicationLogService = applicationLogService;
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

            List<EnderecoArmazenagem> enderecosPontoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.PesquisarPorPontoArmazenagem(viewModel.IdPontoArmazenagem.Value);
            bool enderecoExiste = enderecosPontoArmazenagem.Any(w => w.Codigo.Equals(viewModel.Codigo, StringComparison.OrdinalIgnoreCase));

            if (enderecoExiste)
            {
                Notify.Error("Endereço já existe no ponto de armazenagem.");
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
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            EnderecoArmazenagem enderecoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.GetById(viewModel.IdEnderecoArmazenagem);

            if (!enderecoArmazenagem.Codigo.Equals(viewModel.Codigo, StringComparison.OrdinalIgnoreCase))
            {
                List<EnderecoArmazenagem> enderecosPontoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.PesquisarPorPontoArmazenagem(viewModel.IdPontoArmazenagem.Value);
                int numeroEnderecos = enderecosPontoArmazenagem.Where(w =>
                    w.IdEnderecoArmazenagem != viewModel.IdEnderecoArmazenagem &&
                    w.Codigo.Equals(viewModel.Codigo, StringComparison.OrdinalIgnoreCase)).Count();

                if (numeroEnderecos > 0)
                {
                    Notify.Error("Endereço já existe no ponto de armazenagem.");
                    return View(viewModel);
                }
            }

            enderecoArmazenagem.Ativo = viewModel.Ativo;
            enderecoArmazenagem.IsEntrada = viewModel.IsEntrada;
            enderecoArmazenagem.Codigo = viewModel.Codigo;
            enderecoArmazenagem.EstoqueMaximo = viewModel.EstoqueMaximo;
            enderecoArmazenagem.EstoqueMinimo = viewModel.EstoqueMinimo;
            enderecoArmazenagem.IdNivelArmazenagem = viewModel.IdNivelArmazenagem.Value;
            enderecoArmazenagem.IdPontoArmazenagem = viewModel.IdPontoArmazenagem.Value;
            enderecoArmazenagem.IsFifo = viewModel.IsFifo;
            enderecoArmazenagem.IsPontoSeparacao = viewModel.IsPontoSeparacao;
            enderecoArmazenagem.LimitePeso = viewModel.LimitePeso;
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

            var loteProdutoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarRegistrosPorEndereco(viewModel.IdEnderecoArmazenagem);

            // Popula Itens na lista de produtos do Endereço Armazenagem
            loteProdutoEndereco.ForEach(lpe =>
            {
                if (lpe.EnderecoArmazenagem.PontoArmazenagem.Descricao != "Picking")
                {
                    var item = new ProdutoItem
                    {
                        NumeroLote = lpe.Lote.IdLote.ToString(),
                        NumeroNf = lpe.Lote.NotaFiscal.Numero.ToString(),
                        CodigoReferencia = lpe.Produto.Referencia,
                        DataInstalacao = lpe.DataHoraInstalacao.ToString("dd/MM/yyyy HH:mm:ss"),
                        Descricao = lpe.Produto.Descricao,
                        Multiplo = lpe.Produto.MultiploVenda.ToString(),
                        Peso = lpe.Produto.PesoBruto.ToString("n2"),
                        QuantidadeInstalada = lpe.Quantidade.ToString(),
                        UsuarioResponsavel = _unitOfWork.PerfilUsuarioRepository.GetByUserId(lpe.AspNetUsers.Id).Nome
                    };

                    viewModel.Items.Add(item);
                }
                else
                {
                    var item = new ProdutoItem
                    {
                        CodigoReferencia = lpe.Produto.Referencia,
                        DataInstalacao = lpe.DataHoraInstalacao.ToString("dd/MM/yyyy HH:mm:ss"),
                        Descricao = lpe.Produto.Descricao,
                        Multiplo = lpe.Produto.MultiploVenda.ToString("n2"),
                        Peso = lpe.Produto.PesoBruto.ToString("n2"),
                        QuantidadeInstalada = lpe.Quantidade.ToString(),
                        UsuarioResponsavel = _unitOfWork.PerfilUsuarioRepository.GetByUserId(lpe.AspNetUsers.Id).Nome
                    };

                    viewModel.Items.Add(item);
                }
            });

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult ConfirmarImpressao(long IdEnderecoArmazenagem)
        {
            var endereco = _unitOfWork.EnderecoArmazenagemRepository.GetById(IdEnderecoArmazenagem);

            return View(new EnderecoArmazenagemConfirmaImpressaoViewModel
            {
                IdEnderecoArmazenagem = endereco.IdEnderecoArmazenagem,
                Codigo = endereco.Codigo
            });
        }

        [HttpPost]
        public JsonResult ImprimirEtiqueta(BOImprimirTermoResponsabilidadeViewModel viewModel)
        {
            try
            {
                ValidateModel(viewModel);

                var request = new ImprimirEtiquetaEnderecoRequest
                {
                    IdEnderecoArmazenagem = viewModel.IdEnderecoArmazenagem,
                    IdImpressora = viewModel.IdImpressora
                };

                _etiquetaService.ImprimirEtiquetaEndereco(request);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Impressão enviada com sucesso."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _applicationLogService.Error(ApplicationEnum.BackOffice, ex);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Ocorreu um erro na impressão."
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult PesquisaModal(long? id)
        {
            var model = new EnderecoArmazenagemPesquisaModalViewModel();

            model.Filtros.IdPontoArmazenagem = id;

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize]
        public ActionResult EnderecoArmazenagemPesquisaModalDadosLista(DataTableFilter<EnderecoArmazenagemPesquisaModalFiltroViewModel> model)
        {
            var filtros = Mapper.Map<DataTableFilter<EnderecoArmazenagemPesquisaModalFiltro>>(model);
            filtros.CustomFilter.IdEmpresa = IdEmpresa;

            IEnumerable<EnderecoArmazenagemPesquisaModalListaLinhaTabela> result = _unitOfWork.EnderecoArmazenagemRepository.BuscarListaModal(filtros, out int registrosFiltrados, out int totalRegistros);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = Mapper.Map<IEnumerable<EnderecoArmazenagemPesquisaModalItemViewModel>>(result)
            });
        }
    }
}
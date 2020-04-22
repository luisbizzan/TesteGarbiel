using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using System.Collections.Generic;
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

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Caixa.Listar)]
        public ActionResult Index()
        {
            SetViewBags();

            return View();
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Caixa.Listar)]
        public ActionResult DadosLista(DataTableFilter<CaixaListaFiltro> model)
        {
            model.CustomFilter.IdEmpresa = IdEmpresa;

            var result = _caixaService.BuscarLista(model, out int registrosFiltrados, out int totalRegistros);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = Mapper.Map<IEnumerable<CaixaListaTabela>>(result)
            });
        }

        //[HttpGet]
        //[ApplicationAuthorize(Permissions = Permissions.Caixa.Cadastrar)]
        //public ActionResult Cadastrar()
        //{
        //    var viewModel = new CaixaCadastroViewModel
        //    {
        //        Ativo = true
        //    };

        //    return View(viewModel);
        //}

        //[HttpPost]
        //[ApplicationAuthorize(Permissions = Permissions.Caixa.Cadastrar)]
        //public ActionResult Cadastrar(CaixaCadastroViewModel viewModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(viewModel);
        //    }

        //    List<Caixa> enderecosPontoArmazenagem = _unitOfWork.CaixaRepository.PesquisarPorPontoArmazenagem(viewModel.IdPontoArmazenagem.Value);
        //    bool enderecoExiste = enderecosPontoArmazenagem.Any(w => w.Codigo.Equals(viewModel.Codigo, StringComparison.OrdinalIgnoreCase));

        //    if (enderecoExiste)
        //    {
        //        Notify.Error("Endereço já existe no ponto de armazenagem.");
        //        return View(viewModel);
        //    }

        //    var Caixa = Mapper.Map<Caixa>(viewModel);
        //    Caixa.IdEmpresa = IdEmpresa;

        //    _caixaService.Cadastrar(Caixa);

        //    Notify.Success("Endereço de Armazenagem cadastrado com sucesso.");
        //    return RedirectToAction("Index");
        //}

        //[HttpPost]
        //[ApplicationAuthorize(Permissions = Permissions.Caixa.Excluir)]
        //public JsonResult ExcluirAjax(int id)
        //{
        //    try
        //    {
        //        _CaixaService.Excluir(id);

        //        return Json(new AjaxGenericResultModel
        //        {
        //            Success = true,
        //            Message = Resources.CommonStrings.RegisterDeletedSuccessMessage
        //        }, JsonRequestBehavior.DenyGet);
        //    }
        //    catch
        //    {
        //        return Json(new AjaxGenericResultModel
        //        {
        //            Success = false,
        //            Message = Resources.CommonStrings.RegisterHasRelationshipsErrorMessage
        //        }, JsonRequestBehavior.DenyGet);
        //    }
        //}

        //[HttpGet]
        //[ApplicationAuthorize(Permissions = Permissions.Caixa.Editar)]
        //public ActionResult Editar(long id)
        //{
        //    Caixa Caixa = _unitOfWork.CaixaRepository.GetById(id);

        //    var viewModel = Mapper.Map<CaixaEditarViewModel>(Caixa);

        //    return View(viewModel);
        //}

        //[HttpPost]
        //[ApplicationAuthorize(Permissions = Permissions.Caixa.Editar)]
        //public ActionResult Editar(CaixaEditarViewModel viewModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(viewModel);
        //    }

        //    Caixa Caixa = _unitOfWork.CaixaRepository.GetById(viewModel.IdCaixa);

        //    if (!Caixa.Codigo.Equals(viewModel.Codigo, StringComparison.OrdinalIgnoreCase))
        //    {
        //        List<Caixa> enderecosPontoArmazenagem = _unitOfWork.CaixaRepository.PesquisarPorPontoArmazenagem(viewModel.IdPontoArmazenagem.Value);
        //        int numeroEnderecos = enderecosPontoArmazenagem.Where(w =>
        //            w.IdCaixa != viewModel.IdCaixa &&
        //            w.Codigo.Equals(viewModel.Codigo, StringComparison.OrdinalIgnoreCase)).Count();

        //        if (numeroEnderecos > 0)
        //        {
        //            Notify.Error("Endereço já existe no ponto de armazenagem.");
        //            return View(viewModel);
        //        }
        //    }

        //    Caixa.Ativo = viewModel.Ativo;
        //    Caixa.IsEntrada = viewModel.IsEntrada;
        //    Caixa.Codigo = viewModel.Codigo;
        //    Caixa.EstoqueMaximo = viewModel.EstoqueMaximo;
        //    Caixa.EstoqueMinimo = viewModel.EstoqueMinimo;
        //    Caixa.IdNivelArmazenagem = viewModel.IdNivelArmazenagem.Value;
        //    Caixa.IdPontoArmazenagem = viewModel.IdPontoArmazenagem.Value;
        //    Caixa.IsFifo = viewModel.IsFifo;
        //    Caixa.IsPontoSeparacao = viewModel.IsPontoSeparacao;
        //    Caixa.LimitePeso = viewModel.LimitePeso;
        //    Caixa.IdEmpresa = IdEmpresa;

        //    _CaixaService.Editar(Caixa);

        //    Notify.Success("Endereço de Armazenagem editado com sucesso.");
        //    return RedirectToAction("Index");
        //}

        //[HttpGet]
        //[ApplicationAuthorize(Permissions = Permissions.Caixa.Visualizar)]
        //public ActionResult Detalhes(long id)
        //{
        //    Caixa Caixa = _unitOfWork.CaixaRepository.GetById(id);

        //    var viewModel = Mapper.Map<CaixaDetalhesViewModel>(Caixa);

        //    var loteProdutoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarRegistrosPorEndereco(viewModel.IdCaixa);

        //    // Popula Itens na lista de produtos do Endereço Armazenagem
        //    loteProdutoEndereco.ForEach(lpe =>
        //    {
        //        if (lpe.Caixa.PontoArmazenagem.Descricao != "Picking")
        //        {
        //            var item = new ProdutoItem
        //            {
        //                NumeroLote = lpe.Lote == null ? "-" : lpe.Lote.IdLote.ToString(),
        //                NumeroNf = lpe.Lote == null ? "-" : lpe.Lote.NotaFiscal.Numero.ToString(),
        //                CodigoReferencia = lpe.Produto.Referencia,
        //                DataInstalacao = lpe.DataHoraInstalacao.ToString("dd/MM/yyyy HH:mm:ss"),
        //                Descricao = lpe.Produto.Descricao,
        //                Multiplo = lpe.Produto.MultiploVenda.ToString(),
        //                Peso = lpe.Produto.PesoBruto.ToString("n2"),
        //                QuantidadeInstalada = lpe.Quantidade.ToString(),
        //                UsuarioResponsavel = _unitOfWork.PerfilUsuarioRepository.GetByUserId(lpe.AspNetUsers.Id).Nome
        //            };

        //            viewModel.Items.Add(item);
        //        }
        //        else
        //        {
        //            var item = new ProdutoItem
        //            {
        //                CodigoReferencia = lpe.Produto.Referencia,
        //                DataInstalacao = lpe.DataHoraInstalacao.ToString("dd/MM/yyyy HH:mm:ss"),
        //                Descricao = lpe.Produto.Descricao,
        //                Multiplo = lpe.Produto.MultiploVenda.ToString("n2"),
        //                Peso = lpe.Produto.PesoBruto.ToString("n2"),
        //                QuantidadeInstalada = lpe.Quantidade.ToString(),
        //                UsuarioResponsavel = _unitOfWork.PerfilUsuarioRepository.GetByUserId(lpe.AspNetUsers.Id).Nome
        //            };

        //            viewModel.Items.Add(item);
        //        }
        //    });

        //    return View(viewModel);
        //}

        //[HttpGet]
        //public ActionResult PesquisaModal(long? id, bool? buscarTodos)
        //{
        //    var model = new CaixaPesquisaModalViewModel();

        //    model.Filtros.IdPontoArmazenagem = id;
        //    model.Filtros.BuscarTodos = buscarTodos ?? false;

        //    return View(model);
        //}

        //[HttpPost]
        //[ApplicationAuthorize]
        //public ActionResult CaixaPesquisaModalDadosLista(DataTableFilter<CaixaPesquisaModalFiltroViewModel> model)
        //{
        //    var filtros = Mapper.Map<DataTableFilter<CaixaPesquisaModalFiltro>>(model);
        //    filtros.CustomFilter.IdEmpresa = IdEmpresa;

        //    IEnumerable<CaixaPesquisaModalListaLinhaTabela> result = _unitOfWork.CaixaRepository.BuscarListaModal(filtros, out int registrosFiltrados, out int totalRegistros);

        //    return DataTableResult.FromModel(new DataTableResponseModel
        //    {
        //        Draw = model.Draw,
        //        RecordsTotal = totalRegistros,
        //        RecordsFiltered = registrosFiltrados,
        //        Data = Mapper.Map<IEnumerable<CaixaPesquisaModalItemViewModel>>(result)
        //    });
        //}
    }
}
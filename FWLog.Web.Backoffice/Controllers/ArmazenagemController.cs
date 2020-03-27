using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.ArmazenagemCtx;
using FWLog.Web.Backoffice.Models.CommonCtx;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class ArmazenagemController : BOBaseController
    {
        private readonly UnitOfWork _uow;
        private readonly RelatorioService _relatorioService;

        public ArmazenagemController(UnitOfWork uow, RelatorioService relatorioService)
        {
            _uow = uow;
            _relatorioService = relatorioService;
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioRastreabilidadeLote)]
        public ActionResult RelatorioRastreabilidadeLote()
        {
            return View();
        }

        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioRastreabilidadeLote)]
        public ActionResult RelatorioRastreabilidadeLotePageData(DataTableFilter<RelatorioRastreabilidadeLoteFilterViewModel> model)
        {
            var filtro = Mapper.Map<DataTableFilter<RastreabilidadeLoteListaFiltro>>(model);
            filtro.CustomFilter.IdEmpresa = IdEmpresa;

            var result = _uow.LoteProdutoRepository.PesquisarPorLoteOuProduto(filtro, out int registrosFiltrados, out int totalRegistros);

            var list = new List<RelatorioRastreabilidadeLoteListItemViewModel>();

            foreach (var item in result)
            {
                list.Add(new RelatorioRastreabilidadeLoteListItemViewModel()
                {
                    IdLote = item.IdLote,
                    Status = item.Status,
                    DataRecebimento = item.DataRecebimento.ToString("dd/MM/yyyy"),
                    DataConferencia = item.DataConferencia.HasValue ? item.DataConferencia.Value.ToString("dd/MM/yyyy") : "",
                    QuantidadeVolume = item.QuantidadeVolume.ToString(),
                    QuantidadePeca = item.QuantidadePeca.ToString()
                });
            }

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = list
            });
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioRastreabilidadeLote)]
        public ActionResult RelatorioRastreabilidadeLoteProduto(long idLote)
        {
            var model = new RelatorioRastreabilidadeLoteProdutoViewModel
            {
                Filter = new RelatorioRastreabilidadeLoteProdutoFilterViewModel()
            };

            model.Filter.IdLote = idLote;

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioRastreabilidadeLote)]
        public ActionResult RelatorioRastreabilidadeLoteProdutoPageData(DataTableFilter<RelatorioRastreabilidadeLoteProdutoFilterViewModel> model)
        {
            var filtro = Mapper.Map<DataTableFilter<RastreabilidadeLoteProdutoListaFiltro>>(model);
            filtro.CustomFilter.IdEmpresa = IdEmpresa;

            var result = _uow.LoteProdutoRepository.ConsultarPorLote(filtro, out int registrosFiltrados, out int totalRegistros);

            var list = new List<RelatorioRastreabilidadeLoteProdutoListItemViewModel>();

            foreach (var item in result)
            {
                list.Add(new RelatorioRastreabilidadeLoteProdutoListItemViewModel()
                {
                    IdProduto = item.IdProduto,
                    IdLote = item.IdLote,
                    ReferenciaProduto = item.ReferenciaProduto,
                    DescricaoProduto = item.DescricaoProduto,
                    QuantidadeRecebida = item.QuantidadeRecebida.ToString(),
                    Saldo = item.Saldo.ToString()
                });
            }

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = list
            });
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioRastreabilidadeLote)]
        public ActionResult RelatorioRastreabilidadeLoteMovimentacao(long idLote, long idProduto)
        {
            var model = new RelatorioRastreabilidadeLoteMovimentacaoViewModel
            {
                Filter = new RelatorioRastreabilidadeLoteMovimentacaoFilterViewModel()
                {
                    ListaLoteMovimentacaoTipo = new SelectList(
                    _uow.LoteMovimentacaoTipoRepository.Todos().OrderBy(o => o.IdLoteMovimentacaoTipo).Select(x => new SelectListItem
                    {
                        Value = x.IdLoteMovimentacaoTipo.GetHashCode().ToString(),
                        Text = x.Descricao,
                    }), "Value", "Text"
                )
                }
            };

            model.Filter.IdLote = idLote;
            model.Filter.IdProduto = idProduto;
            model.Filter.DescricaoProduto = _uow.ProdutoRepository.GetById(idProduto) != null ? _uow.ProdutoRepository.GetById(idProduto).Descricao : "";

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioRastreabilidadeLote)]
        public ActionResult RelatorioRastreabilidadeLoteMovimentacaoPageData(DataTableFilter<RelatorioRastreabilidadeLoteMovimentacaoFilterViewModel> model)
        {
            var filtro = Mapper.Map<DataTableFilter<RastreabilidadeLoteMovimentacaoListaFiltro>>(model);
            filtro.CustomFilter.IdEmpresa = IdEmpresa;

            var result = _uow.LoteMovimentacaoRepository.ConsultarPorLoteEProduto(filtro, out int registrosFiltrados, out int totalRegistros);

            var list = new List<RelatorioRastreabilidadeLoteMovimentacaoListItemViewModel>();
            List<UsuarioEmpresa> usuarios = _uow.UsuarioEmpresaRepository.ObterPorEmpresa(IdEmpresa);

            foreach (var item in result)
            {
                list.Add(new RelatorioRastreabilidadeLoteMovimentacaoListItemViewModel()
                {
                    IdProduto = item.IdProduto,
                    IdLote = item.IdLote,
                    ReferenciaProduto = item.ReferenciaProduto,
                    DescricaoProduto = item.DescricaoProduto,
                    UsuarioMovimentacao = usuarios.Where(x => x.UserId.Equals(item.IdUsuarioMovimentacao)).FirstOrDefault()?.PerfilUsuario.Nome,
                    Tipo = item.Tipo.ToString(),
                    Endereco = item.Endereco,
                    Quantidade = item.Quantidade.ToString(),
                    DataHora = item.DataHora.ToString("dd/MM/yyyy hh:mm:ss")
                });
            }

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = list
            });
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioLoteMovimentacao)]
        public ActionResult RelatorioLoteMovimentacao()
        {
            var model = new RelatorioLoteMovimentacaoViewModel
            {
                Filter = new RelatorioLoteMovimentacaoFilterViewModel()
                {
                    ListaLoteMovimentacaoTipo = new SelectList(
                    _uow.LoteMovimentacaoTipoRepository.Todos().OrderBy(o => o.IdLoteMovimentacaoTipo).Select(x => new SelectListItem
                    {
                        Value = x.IdLoteMovimentacaoTipo.GetHashCode().ToString(),
                        Text = x.Descricao,
                    }), "Value", "Text"
                )
                }
            };

            return View(model); ;
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosArmazenagem.RelatorioLoteMovimentacao)]
        public ActionResult RelatorioLoteMovimentacaoPageData(DataTableFilter<RelatorioLoteMovimentacaoFilterViewModel> model)
        {
            var filtro = Mapper.Map<DataTableFilter<LoteMovimentacaoListaFiltro>>(model);
            filtro.CustomFilter.IdEmpresa = IdEmpresa;

            var result = _uow.LoteMovimentacaoRepository.Consultar(filtro, out int registrosFiltrados, out int totalRegistros);

            var list = new List<RelatorioLoteMovimentacaoListItemViewModel>();
            List<UsuarioEmpresa> usuarios = _uow.UsuarioEmpresaRepository.ObterPorEmpresa(IdEmpresa);

            foreach (var item in result)
            {
                list.Add(new RelatorioLoteMovimentacaoListItemViewModel()
                {
                    IdProduto = item.IdProduto,
                    IdLote = item.IdLote,
                    ReferenciaProduto = item.ReferenciaProduto,
                    DescricaoProduto = item.DescricaoProduto,
                    UsuarioMovimentacao = usuarios.Where(x => x.UserId.Equals(item.IdUsuarioMovimentacao)).FirstOrDefault()?.PerfilUsuario.Nome,
                    Tipo = item.Tipo.ToString(),
                    Endereco = item.Endereco,
                    Quantidade = item.Quantidade.ToString(),
                    DataHora = item.DataHora.ToString("dd/MM/yyyy hh:mm:ss")
                });
            }

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = list
            });
        }
    }
}
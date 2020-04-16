using DartDigital.Library.Exceptions;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class AtividadeEstoqueRepository : GenericRepository<AtividadeEstoque>
    {
        public AtividadeEstoqueRepository(Entities entities) : base(entities) { }

        public AtividadeEstoque Pesquisar(long idEmpresa, AtividadeEstoqueTipoEnum idAtividadeEstoqueTipo, long idEnderecoArmazenagem, long idProduto, bool finalizado)
        {
            return Entities.AtividadeEstoque.Where(x => x.IdEmpresa == idEmpresa && x.IdAtividadeEstoqueTipo == idAtividadeEstoqueTipo && x.IdEnderecoArmazenagem == idEnderecoArmazenagem &&
            x.IdProduto == idProduto && x.Finalizado == finalizado).FirstOrDefault();
        }

        public List<AtividadeEstoqueListaLinhaTabela> PesquisarAtividade(long idEmpresa, string idUsuario, List<int> tiposAtividade)
        {
            UsuarioEmpresa empresaUsuario = Entities.UsuarioEmpresa.Where(w => w.IdEmpresa == idEmpresa && w.UserId == idUsuario).FirstOrDefault();

            if (empresaUsuario == null)
            {
                throw new BusinessException("O usuário não tem configuração de empresa.");
            }

            if (!empresaUsuario.CorredorEstoqueInicio.HasValue || !empresaUsuario.CorredorEstoqueFim.HasValue)
            {
                return new List<AtividadeEstoqueListaLinhaTabela>();
            }

            var query = (from a in Entities.AtividadeEstoque
                         join e in Entities.EnderecoArmazenagem on a.IdEnderecoArmazenagem equals e.IdEnderecoArmazenagem
                         join p in Entities.Produto on a.IdProduto equals p.IdProduto
                         where
                            a.IdEmpresa == idEmpresa &&
                            !a.Finalizado &&
                            (empresaUsuario.CorredorEstoqueInicio == null || e.Corredor >= empresaUsuario.CorredorEstoqueInicio) &&
                            (empresaUsuario.CorredorEstoqueFim == null || e.Corredor <= empresaUsuario.CorredorEstoqueFim) &&
                            (tiposAtividade.Contains((int)a.IdAtividadeEstoqueTipo))
                         orderby e.Codigo, e.Horizontal, e.Vertical, e.Divisao
                         select new AtividadeEstoqueListaLinhaTabela
                         {
                             IdAtividadeEstoque = a.IdAtividadeEstoque,
                             IdAtividadeEstoqueTipo = (int)a.AtividadeEstoqueTipo.IdAtividadeEstoqueTipo,
                             DescricaoAtividadeEstoqueTipo = a.AtividadeEstoqueTipo.Descricao,
                             IdEnderecoArmazenagem = e.IdEnderecoArmazenagem,
                             IdProduto = p.IdProduto,
                             Referencia = p.Referencia,
                             CodigoEndereco = e.Codigo,
                             Corredor = e.Corredor,
                             Finalizado = a.Finalizado
                         });

            return query.ToList();
        }

        public List<AtividadeEstoque> PesquisarProdutosPendentes(long idEmpresa, AtividadeEstoqueTipoEnum idAtividadeEstoqueTipo, long idProduto)
        {
            return Entities.AtividadeEstoque.Where(x => x.IdEmpresa == idEmpresa &&
                                                            x.IdAtividadeEstoqueTipo == idAtividadeEstoqueTipo &&
                                                            x.IdProduto == idProduto &&
                                                            x.Finalizado == false).ToList();
        }

        public IEnumerable<AtividadeEstoqueListaTabela> PesquisarPageData(DataTableFilter<AtividadeEstoqueListaFiltro> model, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.AtividadeEstoque.Where(w => w.IdEmpresa == model.CustomFilter.IdEmpresa).Count();

            var dataInicialSolicitacao = model.CustomFilter.DataInicialSolicitacao.HasValue ? new DateTime(model.CustomFilter.DataInicialSolicitacao.Value.Year,
                model.CustomFilter.DataInicialSolicitacao.Value.Month, model.CustomFilter.DataInicialSolicitacao.Value.Day, 0, 0, 0) : (DateTime?)null;

            var dataFinalSolicitacao = model.CustomFilter.DataFinalSolicitacao.HasValue ? new DateTime(model.CustomFilter.DataFinalSolicitacao.Value.Year,
                model.CustomFilter.DataFinalSolicitacao.Value.Month, model.CustomFilter.DataFinalSolicitacao.Value.Day, 23, 59, 59) : (DateTime?)null;

            var dataInicialExecucao = model.CustomFilter.DataInicialExecucao.HasValue ? new DateTime(model.CustomFilter.DataInicialExecucao.Value.Year,
                model.CustomFilter.DataInicialExecucao.Value.Month, model.CustomFilter.DataInicialExecucao.Value.Day, 0, 0, 0) : (DateTime?)null;

            var dataFinalExecucao = model.CustomFilter.DataFinalExecucao.HasValue ? new DateTime(model.CustomFilter.DataFinalExecucao.Value.Year,
                model.CustomFilter.DataFinalExecucao.Value.Month, model.CustomFilter.DataFinalExecucao.Value.Day, 23, 59, 59) : (DateTime?)null;

            IQueryable<AtividadeEstoqueListaTabela> query =
                Entities.AtividadeEstoque.AsNoTracking().Where(w => w.IdEmpresa == model.CustomFilter.IdEmpresa &&
                    (model.CustomFilter.IdAtividadeEstoqueTipo.HasValue == false || w.IdAtividadeEstoqueTipo == (AtividadeEstoqueTipoEnum)model.CustomFilter.IdAtividadeEstoqueTipo.Value) &&
                    (model.CustomFilter.QuantidadeInicial.HasValue == false || w.QuantidadeInicial == model.CustomFilter.QuantidadeInicial.Value) &&
                    (model.CustomFilter.QuantidadeFinal.HasValue == false || w.QuantidadeFinal == model.CustomFilter.QuantidadeFinal.Value) &&
                    (dataInicialSolicitacao.HasValue == false || w.DataSolicitacao >= dataInicialSolicitacao) &&
                    (dataFinalSolicitacao.HasValue == false || w.DataSolicitacao <= dataFinalSolicitacao) &&
                    (dataInicialExecucao.HasValue == false || w.DataExecucao >= dataInicialExecucao) &&
                    (dataFinalExecucao.HasValue == false || w.DataExecucao <= dataFinalExecucao) &&
                    (string.IsNullOrEmpty(model.CustomFilter.IdUsuarioExecucao) || w.IdUsuarioExecucao.Contains(model.CustomFilter.IdUsuarioExecucao)) &&
                    (model.CustomFilter.IdProduto.HasValue == false || w.IdProduto == model.CustomFilter.IdProduto.Value))
                .Select(s => new AtividadeEstoqueListaTabela
                {
                    CodigoEndereco = s.EnderecoArmazenagem.Codigo,
                    UsuarioExecucao = s.IdUsuarioExecucao,
                    TipoAtividade = s.AtividadeEstoqueTipo.Descricao,
                    QuantidadeInicial = s.QuantidadeInicial,
                    QuantidadeFinal = s.QuantidadeFinal,
                    DataExecucao = s.DataExecucao,
                    DataSolicitacao = s.DataSolicitacao,
                    ReferenciaProduto = s.Produto.Referencia,
                    DescricaoProduto = s.Produto.Descricao,
                    Finalizado = s.Finalizado
                });

            totalRecordsFiltered = query.Count();

            query = query
                .OrderBy(model.OrderByColumn, model.OrderByDirection)
                .Skip(model.Start)
                .Take(model.Length);

            return query.ToList();
        }
    }
}
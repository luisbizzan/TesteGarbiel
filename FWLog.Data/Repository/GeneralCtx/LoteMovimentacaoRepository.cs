using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class LoteMovimentacaoRepository : GenericRepository<LoteMovimentacao>
    {
        public LoteMovimentacaoRepository(Entities entities) : base(entities) { }

        public IEnumerable<RastreabilidadeLoteMovimentacaoListaLinhaTabela> Consultar(DataTableFilter<LoteMovimentacaoListaFiltro> model, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.LoteMovimentacao.Where(x => x.IdEmpresa == model.CustomFilter.IdEmpresa && x.IdLote == model.CustomFilter.IdLote && x.IdProduto == model.CustomFilter.IdProduto).Count();

            DateTime? dataHoraInicial = model.CustomFilter.DataHoraInicial.HasValue ? new DateTime(model.CustomFilter.DataHoraInicial.Value.Year, model.CustomFilter.DataHoraInicial.Value.Month, model.CustomFilter.DataHoraInicial.Value.Day, 0, 0, 0) : (DateTime?)null;
            DateTime? dataHoraFinal = model.CustomFilter.DataHoraFinal.HasValue ? new DateTime(model.CustomFilter.DataHoraFinal.Value.Year, model.CustomFilter.DataHoraFinal.Value.Month, model.CustomFilter.DataHoraFinal.Value.Day, 23, 59, 59) : (DateTime?)null;

            IQueryable<RastreabilidadeLoteMovimentacaoListaLinhaTabela> query =
                Entities.LoteMovimentacao.AsNoTracking().Where(
                    w => w.IdEmpresa == model.CustomFilter.IdEmpresa &&
                    (model.CustomFilter.IdLote.HasValue == false ||  w.IdLote == model.CustomFilter.IdLote) &&
                    (model.CustomFilter.IdProduto.HasValue == false ||  w.IdProduto == model.CustomFilter.IdProduto) &&
                    (dataHoraInicial.HasValue == false || w.DataHora >= dataHoraInicial) &&
                    (dataHoraFinal.HasValue == false || w.DataHora <= dataHoraFinal) &&
                    (String.IsNullOrEmpty(model.CustomFilter.IdUsuarioMovimentacao) || w.IdUsuarioMovimentacao.Contains(model.CustomFilter.IdUsuarioMovimentacao)) &&
                    (model.CustomFilter.IdEnderecoArmazenagem.HasValue == false ||  w.IdEnderecoArmazenagem == model.CustomFilter.IdEnderecoArmazenagem) &&
                    (model.CustomFilter.IdLoteMovimentacaoTipo.HasValue == false || w.IdLoteMovimentacaoTipo == (LoteMovimentacaoTipoEnum)model.CustomFilter.IdLoteMovimentacaoTipo.Value))
                .Select(s => new RastreabilidadeLoteMovimentacaoListaLinhaTabela
                {
                    IdLote = s.IdLote,
                    IdProduto = s.IdProduto,
                    ReferenciaProduto = s.Produto.Referencia,
                    DescricaoProduto = s.Produto.Descricao,
                    Tipo = s.LoteMovimentacaoTipo.IdLoteMovimentacaoTipo,
                    Quantidade = s.Quantidade,
                    DataHora = s.DataHora,
                    Endereco = s.EnderecoArmazenagem.Codigo,
                    IdUsuarioMovimentacao = s.IdUsuarioMovimentacao
                });

            totalRecordsFiltered = query.Count();

            query = query
                .OrderBy(model.OrderByColumn, model.OrderByDirection)
                .Skip(model.Start)
                .Take(model.Length);

            return query.ToList();
        }

        public IEnumerable<RastreabilidadeLoteMovimentacaoListaLinhaTabela> ConsultarPorLoteEProduto(DataTableFilter<RastreabilidadeLoteMovimentacaoListaFiltro> model, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.LoteMovimentacao.Where(x => x.IdEmpresa == model.CustomFilter.IdEmpresa && x.IdLote == model.CustomFilter.IdLote && x.IdProduto == model.CustomFilter.IdProduto).Count();

            DateTime? dataHoraInicial = model.CustomFilter.DataHoraInicial.HasValue ? new DateTime(model.CustomFilter.DataHoraInicial.Value.Year, model.CustomFilter.DataHoraInicial.Value.Month, model.CustomFilter.DataHoraInicial.Value.Day, 0, 0, 0) : (DateTime?)null;
            DateTime? dataHoraFinal = model.CustomFilter.DataHoraFinal.HasValue ?  new DateTime(model.CustomFilter.DataHoraFinal.Value.Year, model.CustomFilter.DataHoraFinal.Value.Month, model.CustomFilter.DataHoraFinal.Value.Day, 23, 59, 59) : (DateTime?)null; 

            IQueryable<RastreabilidadeLoteMovimentacaoListaLinhaTabela> query =
                Entities.LoteMovimentacao.AsNoTracking().Where(
                    w => w.IdEmpresa == model.CustomFilter.IdEmpresa &&
                    (w.IdLote == model.CustomFilter.IdLote) &&
                    (w.IdProduto == model.CustomFilter.IdProduto) &&
                    (String.IsNullOrEmpty(model.CustomFilter.IdUsuarioMovimentacao) || w.IdUsuarioMovimentacao.Contains(model.CustomFilter.IdUsuarioMovimentacao)) &&
                    (model.CustomFilter.QuantidadeInicial.HasValue == false || w.Quantidade >= model.CustomFilter.QuantidadeInicial.Value) &&
                    (model.CustomFilter.QuantidadeFinal.HasValue == false || w.Quantidade <= model.CustomFilter.QuantidadeFinal.Value) &&
                    (dataHoraInicial.HasValue == false || w.DataHora >= dataHoraInicial) &&
                    (dataHoraFinal.HasValue == false || w.DataHora <= dataHoraFinal) &&
                    (model.CustomFilter.IdLoteMovimentacaoTipo.HasValue == false || w.IdLoteMovimentacaoTipo == (LoteMovimentacaoTipoEnum)model.CustomFilter.IdLoteMovimentacaoTipo.Value))
                .Select(s => new RastreabilidadeLoteMovimentacaoListaLinhaTabela
                {
                    IdLote = s.IdLote,
                    IdProduto = s.IdProduto,
                    ReferenciaProduto = s.Produto.Referencia,
                    DescricaoProduto = s.Produto.Descricao,
                    Tipo = s.LoteMovimentacaoTipo.IdLoteMovimentacaoTipo,
                    Quantidade = s.Quantidade,
                    DataHora = s.DataHora,
                    Endereco = s.EnderecoArmazenagem.Codigo,
                    IdUsuarioMovimentacao = s.IdUsuarioMovimentacao
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

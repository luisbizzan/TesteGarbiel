using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class LoteProdutoRepository : GenericRepository<LoteProduto>
    {
        public LoteProdutoRepository(Entities entities) : base(entities) { }

        public LoteProduto ConsultarPorLote(long idLote)
        {
            return Entities.LoteProduto.Where(x => x.IdLote == idLote).FirstOrDefault();
        }

        public IEnumerable<RastreabilidadeLoteProdutoListaLinhaTabela> ConsultarPorLote(DataTableFilter<RastreabilidadeLoteProdutoListaFiltro> model, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.LoteProduto.Where(x => x.IdEmpresa == model.CustomFilter.IdEmpresa).Count();

            IQueryable<RastreabilidadeLoteProdutoListaLinhaTabela> query =
                Entities.LoteProduto.AsNoTracking().Where(w => w.IdEmpresa == model.CustomFilter.IdEmpresa &&
                    (w.IdLote == model.CustomFilter.IdLote) &&
                    (model.CustomFilter.IdProduto.HasValue == false || w.IdProduto == model.CustomFilter.IdProduto.Value))
                    .GroupBy(x => new { x.IdLote, x.IdProduto, x.QuantidadeRecebida, x.Saldo, x.Produto })
                .Select(s => new RastreabilidadeLoteProdutoListaLinhaTabela
                {
                    IdLote = s.Key.IdLote,
                    ReferenciaProduto = s.Key.Produto.Referencia,
                    DescricaoProduto = s.Key.Produto.Descricao,
                    IdProduto = s.Key.IdProduto,
                    QuantidadeRecebida = s.Key.QuantidadeRecebida,
                    Saldo = s.Key.Saldo
                });

            totalRecordsFiltered = query.Count();

            query = query
                .OrderBy(model.OrderByColumn, model.OrderByDirection)
                .Skip(model.Start)
                .Take(model.Length);

            return query.ToList();
        }

        public LoteProduto ConsultarPorLoteProduto(long idLote, long idProduto)
        {
            return Entities.LoteProduto.Where(loteProduto => loteProduto.IdLote == idLote && loteProduto.IdProduto == idProduto).FirstOrDefault();
        }

        public LoteProduto PesquisarProdutoNoLote(long idEmpresa, long idLote, long idProduto)
        {
            return Entities.LoteProduto.Where(w => w.IdEmpresa == idEmpresa && w.IdLote == idLote && w.IdProduto == idProduto).FirstOrDefault();
        }

        public LoteProduto PesquisarProdutoMaisAntigoPorEmpresaESaldo(long idProduto, long idEmpresa)
        {
            return Entities.LoteProduto.Where(loteProduto => loteProduto.IdProduto == idProduto &&
                                                                loteProduto.IdEmpresa == idEmpresa &&
                                                                loteProduto.Saldo > 0)
                .OrderBy(loteProduto => loteProduto.Lote.DataRecebimento)
                .FirstOrDefault();
        }

        public IEnumerable<RastreabilidadeLoteListaLinhaTabela> PesquisarPorLoteOuProduto(DataTableFilter<RastreabilidadeLoteListaFiltro> model, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.LoteProduto.Where(w => w.IdEmpresa == model.CustomFilter.IdEmpresa).GroupBy(x => x.IdLote).Count();

            IQueryable<RastreabilidadeLoteListaLinhaTabela> query =
                Entities.LoteProduto.AsNoTracking().Where(w => w.IdEmpresa == model.CustomFilter.IdEmpresa &&
                    (model.CustomFilter.IdLote.HasValue == false || w.IdLote == model.CustomFilter.IdLote.Value) &&
                    (model.CustomFilter.IdProduto.HasValue == false || w.IdProduto == model.CustomFilter.IdProduto.Value))
                    .GroupBy(x => new { x.IdLote, x.Lote })
                .Select(s => new RastreabilidadeLoteListaLinhaTabela
                {
                    IdLote = s.Key.IdLote,
                    Status = s.Key.Lote.LoteStatus.Descricao,
                    DataRecebimento = s.Key.Lote.DataRecebimento,
                    DataConferencia = s.Key.Lote.DataFinalConferencia,
                    QuantidadeVolume = s.Key.Lote.QuantidadeVolume,
                    QuantidadePeca = s.Key.Lote.QuantidadePeca
                });

            totalRecordsFiltered = query.Count();

            query = query
                .OrderBy(model.OrderByColumn, model.OrderByDirection)
                .Skip(model.Start)
                .Take(model.Length);

            return query.ToList();
        }

        public List<RelatorioValidadePecaListaTabela> BuscarListaRelatorioValidadePeca(DataTableFilter<RelatorioValidadePecaListaFiltro> filtro, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.LoteProduto.Where(w => w.IdEmpresa == filtro.CustomFilter.IdEmpresa && w.Saldo > 0 && w.DataValidade.HasValue).Count();

            var query = Entities.LoteProduto.AsNoTracking().Where(w => w.IdEmpresa == filtro.CustomFilter.IdEmpresa && w.Saldo > 0 && w.DataValidade.HasValue);

            if (filtro.CustomFilter.IdLote.HasValue)
            {
                query = query.Where(lp => lp.IdLote == filtro.CustomFilter.IdLote.Value);
            }

            if (filtro.CustomFilter.IdProduto.HasValue)
            {
                query = query.Where(lp => lp.IdProduto == filtro.CustomFilter.IdProduto);
            }

            if (filtro.CustomFilter.DataInicial.HasValue)
            {
                query = query.Where(lp => lp.DataValidade >= filtro.CustomFilter.DataInicial.Value);
            }

            if (filtro.CustomFilter.DataFinal.HasValue)
            {
                query = query.Where(lp => lp.DataValidade <= filtro.CustomFilter.DataFinal.Value);
            }

            totalRecordsFiltered = query.Count();

            query = query
                .OrderBy(filtro.OrderByColumn, filtro.OrderByDirection)
                .Skip(filtro.Start)
                .Take(filtro.Length);

            var resultado = query.ToList().Select(rvp => new RelatorioValidadePecaListaTabela
            {
                IdLote = rvp.IdLote,
                ReferenciaProduto = rvp.Produto.Referencia,
                DescricaoProduto = rvp.Produto.Descricao,
                DataValidade = rvp.DataValidade.HasValue ? rvp.DataValidade.Value.ToString("dd/MM/yyyy") : "",
                Saldo = rvp.Saldo
            }).ToList();

            return resultado;
        }
    }
}

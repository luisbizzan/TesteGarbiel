﻿using ExtensionMethods.List;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class NivelArmazenagemRepository : GenericRepository<NivelArmazenagem>
    {
        public NivelArmazenagemRepository(Entities entities) : base(entities)
        {
        }

        public List<NivelArmazenagem> RetornarAtivos()
        {
            return Entities.NivelArmazenagem.Where(w => w.Ativo).ToList();
        }

        public IList<NivelArmazenagemTableRow> SearchForDataTable(DataTableFilter<NivelArmazenagemFilter> filter, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.NivelArmazenagem.Count();

            IQueryable<NivelArmazenagemTableRow> query = Entities.NivelArmazenagem.AsNoTracking()
                .Select(e => new NivelArmazenagemTableRow
                {
                    IdNivelArmazenagem = e.IdNivelArmazenagem,
                    Ativo = e.Ativo,
                    Descricao = e.Descricao
                });

            query = query.WhereIf(!string.IsNullOrEmpty(filter.CustomFilter.Descricao), x => x.Descricao.Contains(filter.CustomFilter.Descricao));
            query = query.WhereIf(filter.CustomFilter.Ativo.HasValue, x => x.Ativo == filter.CustomFilter.Ativo);

            // Quantidade total de registros com filtros aplicados, sem Skip() e Take().
            totalRecordsFiltered = query.Count();

            return query.PaginationResult(filter);
        }

        public IList<NivelArmazenagemPesquisaModalListaLinhaTabela> BuscarListaModal(DataTableFilter<NivelArmazenagemPesquisaModalFiltro> filtros, out int registrosFiltrados, out int totalRegistros)
        {
            totalRegistros = Entities.NivelArmazenagem.Count(w => w.IdEmpresa == filtros.CustomFilter.IdEmpresa);

            var query = Entities.NivelArmazenagem
                .Where(w => w.IdEmpresa == filtros.CustomFilter.IdEmpresa && 
                (filtros.CustomFilter.Descricao.Equals(string.Empty) || w.Descricao.Contains(filtros.CustomFilter.Descricao)) &&
                (filtros.CustomFilter.Ativo.HasValue == false || w.Ativo == filtros.CustomFilter.Ativo.Value))
                .Select(s => new NivelArmazenagemPesquisaModalListaLinhaTabela
                {
                    IdNivelArmazenagem = s.IdNivelArmazenagem,
                    Descricao = s.Descricao,
                    Status = s.Ativo ? "Ativo" : "Inativo"
                });

            registrosFiltrados = query.Count();

            query = query
                .OrderBy(filtros.OrderByColumn, filtros.OrderByDirection)
                .Skip(filtros.Start)
                .Take(filtros.Length);

            return query.ToList();
        }
    }
}

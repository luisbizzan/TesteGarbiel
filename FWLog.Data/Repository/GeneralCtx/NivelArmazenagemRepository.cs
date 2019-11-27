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
                    IdNivelArmazenagem = e.IdNivelArmazenagem
                    // TODO: Todas as propriedades da classe NivelArmazenagemTableRow devem ser setadas aqui
                });

            // TODO: Todas as propriedades existentes na class NivelArmazenagemFilter deve conter um where aqui, considere o código a seguir como exemplo
            //if (!string.IsNullOrEmpty(filter.CustomFilter.Name))
            //{
            //query = query.Where(x => x.Name.Contains(filter.CustomFilter.Name));
            //}

            // Quantidade total de registros com filtros aplicados, sem Skip() e Take().
            totalRecordsFiltered = query.Count();

            query = query
                .OrderBy(filter.OrderByColumn, filter.OrderByDirection)
                .Skip(filter.Start)
                .Take(filter.Length);

            return query.ToList();
        }

        public IList<NivelArmazenagemPesquisaModalListaLinhaTabela> BuscarListaModal(DataTableFilter<NivelArmazenagemPesquisaModalFiltro> filtros, out int registrosFiltrados, out int totalRegistros)
        {
            totalRegistros = Entities.NivelArmazenagem.Where(w => w.IdEmpresa == filtros.CustomFilter.IdEmpresa).Count();

            var query = Entities.NivelArmazenagem
                .Where(w => 
                w.IdEmpresa == filtros.CustomFilter.IdEmpresa && 
                //(filtros.CustomFilter.Descricao.Equals(string.Empty) || w.Descricao.Contains(filtros.CustomFilter.Descricao)) &&
                (!filtros.CustomFilter.Ativo.HasValue || w.Ativo == filtros.CustomFilter.Ativo.Value))
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

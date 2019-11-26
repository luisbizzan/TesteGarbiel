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
    }
}

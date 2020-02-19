using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class GarantiaRepository : GenericRepository<Garantia>
    {
        public GarantiaRepository(Entities entities) : base(entities)
        {
        }
        public IList<GarantiaTableRow> SearchForDataTable(DataTableFilter<GarantiaFilter> filter, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.Garantia.Count();

            IQueryable<GarantiaTableRow> query = Entities.Garantia.AsNoTracking()
                .Select(e => new GarantiaTableRow
                {
                    IdGarantia = e.IdGarantia,
                    IdCliente = 0,
                    IdTransportadora = e.NotaFiscal.IdTransportadora
                   
                    // TODO: Todas as propriedades da classe GarantiaTableRow devem ser setadas aqui
                });

            // TODO: Todas as propriedades existentes na class GarantiaFilter deve conter um where aqui, considere o código a seguir como exemplo
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

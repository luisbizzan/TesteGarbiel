using ExtensionMethods;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class BOPrinterTypeRepository : GenericRepository<PrinterType>
    {
        public BOPrinterTypeRepository(Entities entities) : base(entities)
        {
        }

        public IList<PrinterTypeTableRow> SearchForDataTable(DataTableFilter<PrinterTypeFilter> filter, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.PrinterType.Count();

            IQueryable<PrinterTypeTableRow> query = Entities.PrinterType.AsNoTracking()
                .Select(e => new PrinterTypeTableRow
                {
                    Id = e.Id,
                    Name = e.Name
                });

            // TODO: Todas as propriedades existentes na class PrinterTypeFilter deve conter um where aqui, considere o código a seguir como exemplo
            // query.WhereIf(!string.IsNullOrEmpty(filter.CustomFilter.Name));

            // Quantidade total de registros com filtros aplicados, sem Skip() e Take().
            totalRecordsFiltered = query.Count();

            return query.PaginationResult(filter);
        }

        public IEnumerable<PrinterType> Todos()
        {
            return Entities.PrinterType;
        }
    }
}

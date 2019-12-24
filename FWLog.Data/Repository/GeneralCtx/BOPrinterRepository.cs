using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class BOPrinterRepository : GenericRepository<Printer>
    {
        public BOPrinterRepository(Entities entities) : base(entities)
        {
        }

        public IQueryable<Printer> Tabela(ReadOnlyCollection<long> IdEmpresas)
        {
            return _dbSet.Include(x => x.Empresa).Include(x => x.PrinterType).Where(x => IdEmpresas.Contains(x.Empresa.IdEmpresa));
        }

        public List<ImpressoraListaLinhaTabela> BuscarLista(DataTableFilter<ImpressoraListaFiltro> model, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.Printer.Where(w => w.CompanyId == model.CustomFilter.IdEmpresa).Count();

            IQueryable<ImpressoraListaLinhaTabela> query =
                Entities.Printer.AsNoTracking().Where(w => w.CompanyId == model.CustomFilter.IdEmpresa &&
                    (model.CustomFilter.Name.Equals(string.Empty) || w.Name.Contains(model.CustomFilter.Name)) &&
                    (model.CustomFilter.PrinterTypeId.HasValue == false || w.PrinterTypeId == model.CustomFilter.PrinterTypeId.Value) &&
                    (model.CustomFilter.Status.HasValue == false || w.Ativa == model.CustomFilter.Status.Value))
                .Select(s => new ImpressoraListaLinhaTabela
                {
                    Id = s.Id,
                    Name = s.Name,
                    Empresa = s.Empresa.NomeFantasia,
                    Status = s.Ativa ? "Ativa" : "Inativa",
                    PrinterType = s.PrinterType.Name
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

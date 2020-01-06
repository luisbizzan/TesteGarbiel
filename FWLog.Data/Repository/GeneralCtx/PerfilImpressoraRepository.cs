using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PerfilImpressoraRepository : GenericRepository<PerfilImpressora>
    {
        public PerfilImpressoraRepository(Entities entities) : base(entities)
        {

        }

        public IList<PerfilImpressoraTableRow> BuscarLista(DataTableFilter<PerfilImpressoraFilter> filtro, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.PerfilImpressora.Count();

            IQueryable<PerfilImpressoraTableRow> query = 
                Entities.PerfilImpressora.AsNoTracking().Where(w => w.IdEmpresa == filtro.CustomFilter.IdEmpresa &&
                    (filtro.CustomFilter.Nome.Equals(string.Empty) || w.Nome.Contains(filtro.CustomFilter.Nome)) &&
                    (filtro.CustomFilter.Status.HasValue == false || w.Ativo == filtro.CustomFilter.Status))
                .Select(e => new PerfilImpressoraTableRow
                {
                    IdPerfilImpressora = e.IdPerfilImpressora,
                    Nome = e.Nome,
                    Status = e.Ativo ? "Ativo" : "Inativo",
                    IdEmpresa = e.IdEmpresa
                });
                        
            totalRecordsFiltered = query.Count();

            query = query
                .OrderBy(filtro.OrderByColumn, filtro.OrderByDirection)
                .Skip(filtro.Start)
                .Take(filtro.Length);

            return query.ToList();
        }

        public List<PerfilImpressora> Todos ()
        {
            return Entities.PerfilImpressora.ToList();
        }
    }
}

using ExtensionMethods.List;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class MotivoLaudoRepository : GenericRepository<MotivoLaudo>
    {
        public MotivoLaudoRepository(Entities entities) : base(entities) { }

        public IEnumerable<MotivoLaudoLinhaTabela> ObterDadosParaDataTable(DataTableFilter<MotivoLaudoFiltro> filter, out int totalRecordsFiltered, out int totalRecords) 
        {
            totalRecords = Entities.MotivoLaudo.Count();

            IQueryable<MotivoLaudoLinhaTabela> query = Entities.MotivoLaudo.AsNoTracking()
                .Where(x => (filter.CustomFilter.IdMotivoLaudo.HasValue == false || x.IdMotivoLaudo == filter.CustomFilter.IdMotivoLaudo) &&
                (filter.CustomFilter.Descricao.Equals(string.Empty) || x.Descricao.Contains(filter.CustomFilter.Descricao)) &&
                (filter.CustomFilter.Status.HasValue == false || x.Ativo == filter.CustomFilter.Status.Value))
                .Select(e => new MotivoLaudoLinhaTabela
                {
                    IdMotivoLaudo = e.IdMotivoLaudo,
                    Status = e.Ativo ? "Ativo" : "Inativo", 
                    Descricao = e.Descricao
                });

            totalRecordsFiltered = query.Count();

            return query.PaginationResult(filter);
        }
    }
}

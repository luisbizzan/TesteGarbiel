using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Repository.CommonCtx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class MotivoLaudoRepository : GenericRepository<MotivoLaudo>
    {
        public MotivoLaudoRepository(Entities entities) : base(entities) { }

        //public IQueryable<MotivoLaudo> Todos()
        //{
        //    return Entities.Lote;
        //}

        //public IList<MotivoLaudoTableRow> Get()
        //{
        //    totalRecords = Entities.NivelArmazenagem.Count(w => w.IdEmpresa == filter.CustomFilter.IdEmpresa);

        //    IQueryable<NivelArmazenagemTableRow> query = Entities.NivelArmazenagem.AsNoTracking()
        //        .Where(x => x.IdEmpresa == filter.CustomFilter.IdEmpresa &&
        //        (filter.CustomFilter.Descricao.Equals(string.Empty) || x.Descricao.Contains(filter.CustomFilter.Descricao)) &&
        //        (filter.CustomFilter.Status.HasValue == false || x.Ativo == filter.CustomFilter.Status.Value))
        //        .Select(e => new NivelArmazenagemTableRow
        //        {
        //            IdNivelArmazenagem = e.IdNivelArmazenagem,
        //            Status = e.Ativo ? "Ativo" : "Inativo",
        //            Descricao = e.Descricao
        //        });

        //    totalRecordsFiltered = query.Count();

        //    return query.PaginationResult(filter);
        //}

        public IEnumerable<MotivoLaudo> ObterTodos()
        {
            return Entities.MotivoLaudo.ToList();
        }
    }
}

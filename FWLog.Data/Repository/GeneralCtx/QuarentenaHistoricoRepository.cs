using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class QuarentenaHistoricoRepository : GenericRepository<QuarentenaHistorico>
    {
        public QuarentenaHistoricoRepository(Entities entities) : base(entities) { }

        public IQueryable<QuarentenaHistorico> Table()
        {
            return Entities.QuarentenaHistorico;
        }

        public IQueryable<QuarentenaHistoricoListRow> QuarentenaHistoricos()
        {
            var query = (from a in Entities.QuarentenaHistorico
                         join b in Entities.PerfilUsuario
                         on a.IdUsuario equals b.UsuarioId
                         into temp
                         from b in temp.DefaultIfEmpty()
                         select new QuarentenaHistoricoListRow
                         {
                             Data = a.Data,
                             Descricao = a.Descricao,
                             NomeUsuario = b.Nome,
                             IdQuarentena = a.IdQuarentena,
                             IdQuarentenaHistorico = a.IdQuarentenaHistorico,
                             IdUsuario = a.IdUsuario
                         });

            return query;
        }

    }
}

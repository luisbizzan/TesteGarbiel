using FWLog.Data.Models;
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


    }
}

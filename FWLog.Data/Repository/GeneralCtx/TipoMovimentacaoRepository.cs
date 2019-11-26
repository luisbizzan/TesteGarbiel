using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class TipoMovimentacaoRepository : GenericRepository<TipoMovimentacao>
    {
        public TipoMovimentacaoRepository(Entities entities) : base(entities)
        {

        }
    }
}

using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class CaixaTipoRepository : GenericRepository<CaixaTipo>
    {
        public CaixaTipoRepository(Entities entities) : base(entities) { }
    }
}
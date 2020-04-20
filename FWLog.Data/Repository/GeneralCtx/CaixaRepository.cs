using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class CaixaRepository : GenericRepository<Caixa>
    {
        public CaixaRepository(Entities entities) : base(entities) { }
    }
}
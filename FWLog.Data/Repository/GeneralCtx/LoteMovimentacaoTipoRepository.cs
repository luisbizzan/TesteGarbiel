using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class LoteMovimentacaoTipoRepository : GenericRepository<LoteMovimentacaoTipo>
    {
        public LoteMovimentacaoTipoRepository(Entities entities) : base(entities) { }

        public IEnumerable<LoteMovimentacaoTipo> Todos()
        {
            return Entities.LoteMovimentacaoTipo.ToList();
        }

    }
}

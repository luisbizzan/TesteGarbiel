using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class IntegracaoTipoRepository : GenericRepository<IntegracaoTipo>
    {
        public IntegracaoTipoRepository(Entities entities) : base(entities) { }

        public List<IntegracaoTipo> Todos()
        {
            return Entities.IntegracaoTipo.ToList();
        }

   
    }
}

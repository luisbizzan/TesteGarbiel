using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class IntegracaoEntidadeRepository : GenericRepository<IntegracaoEntidade>
    {
        public IntegracaoEntidadeRepository(Entities entities) : base(entities) { }

        public List<IntegracaoEntidade> Todos()
        {
            return Entities.IntegracaoEntidade.ToList();
        }

   
    }
}

using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class ColetorAplicacaoRepository : GenericRepository<ColetorAplicacao>
    {
        public ColetorAplicacaoRepository(Entities entities) : base(entities) { }

        public IEnumerable<ColetorAplicacao> Todos()
        {
            return Entities.ColetorAplicacao;
        }
    }
}

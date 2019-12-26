using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class ArmazenagemRepository : GenericRepository<ArmazenagemRepository>
    {
        public ArmazenagemRepository(Entities entities) : base(entities) { }

        public IEnumerable<Armazenagem> Tabela()
        {
            return Entities.Armazenagem;
        }
    }
}

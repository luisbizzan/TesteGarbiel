using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class CaixaTipoRepository : GenericRepository<CaixaTipo>
    {
        public CaixaTipoRepository(Entities entities) : base(entities) { }

        public List<CaixaTipo> Todos()
        {
            return Entities.CaixaTipo.ToList();
        }
    }
}
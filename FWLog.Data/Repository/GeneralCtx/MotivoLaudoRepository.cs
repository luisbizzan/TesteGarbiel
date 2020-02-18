using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class MotivoLaudoRepository : GenericRepository<MotivoLaudo>
    {
        public MotivoLaudoRepository(Entities entities) : base(entities) { }
    }
}

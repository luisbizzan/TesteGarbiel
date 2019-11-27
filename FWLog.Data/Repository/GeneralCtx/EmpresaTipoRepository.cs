﻿using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class EmpresaTipoRepository : GenericRepository<EmpresaTipo>
    {
        public EmpresaTipoRepository(Entities entities) : base(entities)
        {

        }
    }
}
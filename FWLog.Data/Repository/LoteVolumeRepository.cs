﻿using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class LoteVolumeRepository : GenericRepository<LoteVolume>
    {
        public LoteVolumeRepository(Entities entities) : base(entities) { }
    }
}

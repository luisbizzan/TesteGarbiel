using System;

namespace FWLog.Data.Models.FilterCtx
{
    public class RelatorioResumoProducaoFilter
    {
        public string UserId { get; set; }
        public long IdEmpresa { get; set; }
        public DateTime DateMin { get; set; }
        public DateTime? DateMax { get; set; }
    }
}

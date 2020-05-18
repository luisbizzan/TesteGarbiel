using FWLog.Data.Models.DataTablesCtx;
using System.Collections.Generic;

namespace FWLog.Services.Model.Expedicao
{
    public class DespachoTransportadoraResposta
    {
        public long IdTransportadora { get; set; }

        public List<TransportadoraVolumeForaDoca> VolumesForaDoca { get; set; }
    }
}
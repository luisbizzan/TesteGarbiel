using System.Collections.Generic;

namespace FWLog.Services.Model.Expedicao
{
    public class DespachoTransportadoraResposta
    {
        public long IdTransportadora { get; set; }

        public List<DespachoTransportadoraVolumeResposta> VolumesForaDoca { get; set; }
    }

    public class DespachoTransportadoraVolumeResposta
    {
        public long IdPedidoVendaVolume { get; set; }

        public string NumeroPedido { get; set; }

        public string NumeroVolume { get; set; }

        public string EnderecoCodigo { get; set; }
    }
}
using System.Collections.Generic;

namespace FWLog.Services.Model.Transportadora
{
    public class EnderecosTransportadoraResposta
    {
        public long IdTransportadora { get; set; }

        public string NomeTransportadora { get; set; }

        public List<EnderecosTransportadoraVolumeResposta> ListaEnderecos { get; set; }
    }

    public class EnderecosTransportadoraVolumeResposta
    {
        public long IdPedidoVendaVolume { get; set; }

        public string CodigoEndereco { get; set; }

        public long IdEnderecoArmazenagem { get; set; }
    }
}
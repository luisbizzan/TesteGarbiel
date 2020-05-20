using System.Collections.Generic;

namespace FWLog.Services.Model.Transportadora
{
    public class EnderecosPorTransportadoraResposta
    {
        public long IdTransportadora { get; set; }

        public string NomeTransportadora { get; set; }

        public List<EnderecosPorTransportadoraVolumeResposta> ListaEnderecos { get; set; }
    }

    public class EnderecosPorTransportadoraVolumeResposta
    {
        public long IdPedidoVendaVolume { get; set; }

        public string CodigoEndereco { get; set; }

        public long IdEnderecoArmazenagem { get; set; }
    }
}
using System.Collections.Generic;

namespace FWLog.Services.Model.Expedicao
{
    public class EnderecosPorTransportadoraResposta
    {
        public long IdTransportadora { get; set; }

        public string NomeTransportadora { get; set; }

        public List<string> ListaEnderecos { get; set; }
    }
}
namespace FWLog.Data.Models.DataTablesCtx
{
    public class TransportadoraEnderecoListaLinhaTabela
    {
        public string CnpjTransportadora { get; set; }
        public string RazaoSocialTransportadora { get; set; }
        public long IdTransportadora { get; set; }
        public string Codigo { get; set; }
        public long IdEnderecoArmazenagem { get; set; }
        public long IdTransportadoraEndereco { get; set; }
    }
}

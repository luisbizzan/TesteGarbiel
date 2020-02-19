namespace FWLog.Data.Models.DataTablesCtx
{
    public class ClientePesquisaModalLinhaTabela
    {
        public long CodigoIntegracao { get; set; }
        public long IdCliente { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public string CNPJCPF { get; set; }
        public string Classificacao { get; set; }
        public string Status { get; set; }
    }
}

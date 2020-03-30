namespace FWLog.Data.Models.DataTablesCtx
{
    public class EnderecoProdutoListaLinhaTabela
    {
        public long? IdLote { get; set; }
        public long IdEnderecoArmazenagem { get; set; }
        public long IdProduto { get; set; }
        public string Codigo { get; set; }
        public string Referencia { get; set; }
    }
}

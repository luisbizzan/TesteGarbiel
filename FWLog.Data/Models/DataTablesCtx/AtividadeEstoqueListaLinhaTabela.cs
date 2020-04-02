namespace FWLog.Data.Models.DataTablesCtx
{
    public class AtividadeEstoqueListaLinhaTabela
    {
        public long IdAtividadeEstoque { get; set; }
        public long IdEnderecoArmazenagem { get; set; }
        public long IdProduto { get; set; }
        public AtividadeEstoqueTipoEnum IdAtividadeEstoqueTipo { get; set; }
        public int QuantidadeInicial { get; set; }
        public string Referencia { get; set; }
        public string CodigoEndereco { get; set; }
    }
}

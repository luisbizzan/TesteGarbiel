namespace FWLog.Data.Models.DataTablesCtx
{
    public class AtividadeEstoqueListaLinhaTabela
    {
        public long IdAtividadeEstoque { get; set; }
        public long IdEnderecoArmazenagem { get; set; }
        public long IdProduto { get; set; }
        public int IdAtividadeEstoqueTipo { get; set; }
        public int Corredor { get; set; }
        public string DescricaoAtividadeEstoqueTipo { get; set; }
        public string Referencia { get; set; }
        public string CodigoEndereco { get; set; }
        public bool Finalizado { get; set; }
        public long? IdLote { get; set; }
    }
}
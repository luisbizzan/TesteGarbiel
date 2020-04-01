namespace FWLog.Data.Models.DataTablesCtx
{
    public class RastreabilidadeLoteProdutoListaLinhaTabela
    {
        public long IdProduto { get; set; }

        public long IdLote { get; set; }

        public string ReferenciaProduto { get; set; }

        public string DescricaoProduto { get; set; }

        public long QuantidadeRecebida { get; set; }

        public int Saldo { get; set; }
    }
}

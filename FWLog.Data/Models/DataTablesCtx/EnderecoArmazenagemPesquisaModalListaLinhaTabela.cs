namespace FWLog.Data.Models.DataTablesCtx
{
    public class EnderecoArmazenagemPesquisaModalListaLinhaTabela
    {
        public long IdEnderecoArmazenagem { get; set; }
        public string Codigo { get; set; }
        public decimal? LimitePeso { get; set; }
        public string Fifo { get; set; }
        public int? EstoqueMinimo { get; set; }
        public int? EstoqueMaximo { get; set; }
    }
}

namespace FWLog.Data.Models.DataTablesCtx
{
    public class EnderecoArmazenagemListaLinhaTabela
    {
        public string IdEnderecoArmazenagem { get; set; }
        public string NivelArmazenagem { get; set; }
        public string PontoArmazenagem { get; set; }
        public string Codigo { get; set; }
        public string Fifo { get; set; }
        public string PontoSeparacao { get; set; }
        public string Picking { get; set; }
        public int EstoqueMinimo { get; set; }
        public string Status { get; set; }
        public int? Quantidade { get; set; }
        public bool Ocupado { get; set; }
    }
}

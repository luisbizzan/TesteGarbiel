namespace FWLog.Data.Models.DataTablesCtx
{
    public class PontoArmazenagemListaLinhaTabela
    {
        public long IdPontoArmazenagem { get; set; }
        public string NivelArmazenagem { get; set; }
        public string Descricao { get; set; }
        public string TipoArmazenagem { get; set; }
        public string TipoMovimentacao { get; set; }
        public decimal LimitePesoVertical { get; set; }
        public string Status { get; set; }
    }
}

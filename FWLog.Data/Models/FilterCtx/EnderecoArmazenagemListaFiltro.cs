namespace FWLog.Data.Models.FilterCtx
{
    public class EnderecoArmazenagemListaFiltro
    {
        public long IdEmpresa { get; set; }
        public string Codigo { get; set; }
        public long? IdNivelArmazenagem { get; set; }
        public long? IdPontoArmazenagem { get; set; }
        public bool? Status { get; set; }
    }
}

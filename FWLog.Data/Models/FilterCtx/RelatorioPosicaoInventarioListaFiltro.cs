namespace FWLog.Data.Models.FilterCtx
{
    public class RelatorioPosicaoInventarioListaFiltro
    {
        public string Referencia { get; set; }
        public long IdEmpresa { get; set; }
        public long? IdNivelArmazenagem { get; set; }
        public long? IdPontoArmazenagem { get; set; }
        public long? IdProduto { get; set; }
    }
}

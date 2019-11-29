namespace FWLog.Data.Models.FilterCtx
{
    public class PontoArmazenagemPesquisaModalFiltro
    {
        public long IdEmpresa { get; set; }
        public long? IdNivelArmazenagem { get; set; }
        public string Descricao { get; set; }
        public bool? Status { get; set; }
    }
}

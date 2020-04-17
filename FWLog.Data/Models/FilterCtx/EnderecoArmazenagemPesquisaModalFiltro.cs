namespace FWLog.Data.Models.FilterCtx
{
    public class EnderecoArmazenagemPesquisaModalFiltro
    {
        public long IdEmpresa { get; set; }
        public string Codigo { get; set; }
        public long? IdPontoArmazenagem { get; set; }
        public bool? BuscarTodos { get; set; }
    }
}

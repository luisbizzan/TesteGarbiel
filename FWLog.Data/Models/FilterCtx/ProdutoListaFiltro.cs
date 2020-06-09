namespace FWLog.Data.Models.FilterCtx
{
    public class ProdutoListaFiltro
    {
        public long IdEmpresa { get; set; }
        public string Referencia { get; set; }
        public string Descricao { get; set; }
        public string CodigoDeBarras { get; set; }
        public int? ProdutoStatus { get; set; }
        public int? LocacaoSaldo { get; set; }
        public long? IdPontoArmazenagem { get; set; }
        public long? IdNivelArmazenagem { get; set; }
        public long? IdEnderecoArmazenagem { get; set; }
    }
}

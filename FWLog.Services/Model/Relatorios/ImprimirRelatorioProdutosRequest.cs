namespace FWLog.Services.Model.Relatorios
{
    public class ImprimirRelatorioProdutosRequest
    {
        public long IdImpressora { get; set; }
        public long? IdEmpresa { get; set; }
        public string NomeUsuario { get; set; }
        public string Referencia { get; set; }
        public string Descricao { get; set; }
        public string CodigoDeBarras { get; set; }
        public int? ProdutoStatusId { get; set; }
        public string ProdutoStatus { get; set; }
        public string LocacaoSaldo { get; set; }
        public int? LocacaoSaldoId { get; set; }
        public long? IdPontoArmazenagem { get; set; }
        public long? IdNivelArmazenagem { get; set; }
        public long? IdEnderecoArmazenagem { get; set; }
    }
}

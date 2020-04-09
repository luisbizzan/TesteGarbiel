namespace FWLog.Services.Model.Relatorios
{
    public class RelatorioPosicaoInventarioRequest
    {
        public long IdEmpresa { get; set; }
        public string NomeUsuarioRequisicao { get; set; }
        public long? IdNivelArmazenagem { get; set; }
        public long? IdPontoArmazenagem { get; set; }
        public long? IdProduto { get; set; }
    }
}

namespace FWLog.Services.Model.Relatorios
{
    public class ImprimirRelatorioTotalPorAlaRequest
    {
        public long? IdEmpresa { get; set; }
        public string NomeUsuarioRequisicao { get; set; }
        public long IdNivelArmazenagem { get; set; }
        public long IdPontoArmazenagem { get; set; }
        public int? CorredorInicial { get; set; }
        public int? CorredorFinal { get; set; }
        public bool ImprimirVazia { get; set; }
        public int IdImpressora { get; set; }
    }
}

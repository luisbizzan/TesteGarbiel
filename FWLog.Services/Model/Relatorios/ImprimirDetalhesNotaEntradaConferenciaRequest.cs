namespace FWLog.Services.Model.Relatorios
{
    public class ImprimirDetalhesNotaEntradaConferenciaRequest
    {
        public long IdImpressora { get; set; }
        public long IdEmpresa { get; set; }
        public string NomeUsuario { get; set; }
        public long IdNotaFiscal { get; set; }
    }
}

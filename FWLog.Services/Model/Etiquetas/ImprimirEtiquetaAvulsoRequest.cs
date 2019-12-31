namespace FWLog.Services.Model.Etiquetas
{
    public class ImprimirEtiquetaAvulsoRequest
    {
        public int QuantidadeEtiquetas { get; set; }
        public int IdImpressora { get; set; }
        public long IdEmpresa { get; set; }
    }
}

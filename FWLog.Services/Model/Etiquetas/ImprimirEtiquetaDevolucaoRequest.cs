namespace FWLog.Services.Model.Etiquetas
{
    public class ImprimirEtiquetaDevolucaoRequest
    {
        public string Linha1 { get; set; }
        public string Linha2 { get; set; }
        public string Linha3 { get; set; }
        public int QuantidadeEtiquetas { get; set; } = 1;
        public long IdImpressora { get; set; }
    }
}

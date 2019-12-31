namespace FWLog.Services.Model.Etiquetas
{
    public class ImprimirEtiquetaPecaRequest
    {
        public string ReferenciaProduto { get; set; }
        public int QuantidadeEtiquetas { get; set; }
        public int IdImpressora { get; set; }
        public long IdEmpresa { get; set; }
        public decimal? Multiplo { get; set; }
    }
}

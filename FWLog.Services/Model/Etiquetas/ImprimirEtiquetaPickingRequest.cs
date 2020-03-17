namespace FWLog.Services.Model.Etiquetas
{
    public class ImprimirEtiquetaPickingRequest
    {
        public long IdEnderecoArmazenagem { get; set; }
        public long IdProduto { get; set; }
        public int QuantidadeEtiquetas { get; set; } = 1;
        public long IdImpressora { get; set; }
    }
}

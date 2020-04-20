namespace FWLog.Services.Model.Etiquetas
{
    public class ImprimirEtiquetaEnderecoRequest
    {
        public long IdEnderecoArmazenagem { get; set; }
        public int QuantidadeEtiquetas { get; set; } = 1;
        public long IdImpressora { get; set; }
        public string IdUsuario { get; set; }
        public long IdEmpresa { get; set; }
    }
}

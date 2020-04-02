namespace FWLog.Services.Model.Etiquetas
{
    public class ImprimirEtiquetaAvulsoRequest
    {
        public int QuantidadeEtiquetas { get; set; } = 1;
        public int IdImpressora { get; set; }
        public long IdEmpresa { get; set; }
        public string IdUsuario { get; set; }
        public string ReferenciaProduto { get; set; }
    }
}

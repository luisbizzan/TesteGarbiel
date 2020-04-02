namespace FWLog.Services.Model.Etiquetas
{
    public class ImprimirEtiquetaProdutoBase
    {
        public string ReferenciaProduto { get; set; }
        public int QuantidadeEtiquetas { get; set; } = 1;
        public int IdImpressora { get; set; }
        public long IdEmpresa { get; set; }
        public string IdUsuario { get; set; }
    }
}

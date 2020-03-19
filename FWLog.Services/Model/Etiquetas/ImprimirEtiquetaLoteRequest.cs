namespace FWLog.Services.Model.Etiquetas
{
    public class ImprimirEtiquetaLoteRequest
    {
        public long IdLote { get; set; }

        public long IdProduto { get; set; }

        public long IdImpressora { get; set; }

        public int QuantidadeProdutos { get; set; }

        public int QuantidadeEtiquetas { get; set; }

        public string IdUsuario { get; set; }

        public long IdEmpresa { get; set; }
    }
}
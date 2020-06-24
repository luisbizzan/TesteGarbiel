namespace FWLog.Services.Model.Etiquetas
{
    public class ImprimirEtiquetaArmazenagemVolume
    {
        public long NroLote { get; set; }
        public string ReferenciaProduto { get; set; }
        public int QuantidadeEtiquetas { get; set; }
        public int QuantidadePorCaixa { get; set; }
        public string Usuario { get; set; }
        public long IdImpressora { get; set; }
        public long IdEmpresa { get; set; }
        public decimal? Multiplo { get; set; }
        public bool IsReimpressao { get; set; }
    }
}

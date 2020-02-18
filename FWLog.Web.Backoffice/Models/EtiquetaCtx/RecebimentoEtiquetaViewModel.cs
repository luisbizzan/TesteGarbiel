using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.EtiquetaCtx
{
    public class RecebimentoEtiquetaViewModel
    {
        public int? IdImpressora { get; set; }

        [Display(Name = "Tipo Etiqueta")]
        public int TipoEtiquetagem { get; set; }

        [Display(Name = "Número do Lote")]
        public long? NroLote { get; set; }

        [Display(Name = "Quantidade")]
        public int? Quantide { get; set; }
    }
}
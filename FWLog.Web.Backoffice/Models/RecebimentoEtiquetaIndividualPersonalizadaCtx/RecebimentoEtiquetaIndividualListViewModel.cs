using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.RecebimentoEtiquetaIndividualPersonalizadaCtx
{
    public class RecebimentoEtiquetaIndividualPersonalizadaViewModel
    {
        [Display(Name = "Tipo Etiqueta")]
        public int TipoEtiquetagem { get; set; }

        [Display(Name = "Referência")]
        public long? IdProduto { get; set; }
        public string DescricaoProduto { get; set; }

        [Display(Name = "Quantidade")]
        public int? Quantidade { get; set; }
    }
}
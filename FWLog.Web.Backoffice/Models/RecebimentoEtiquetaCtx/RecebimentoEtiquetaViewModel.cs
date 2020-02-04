using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.RecebimentoNotaCtx
{
    public class RecebimentoEtiquetaViewModel
    {
        public int? IdImpressora { get; set; }

        [Display(Name = "Tipo Etiqueta")]
        public int TipoEtiquetagem { get; set; }

        [Display(Name = "Número do Lote")]
        public long? NroLote { get; set; }

        [Required]
        [Display(Name = "Referência do Produto")]
        public string ReferenciaProduto { get; set; }

        [Display(Name = "Quantide por Caixa")]
        public int? QtdPorCaixa { get; set; }

        [Display(Name = "Quantidade de Caixas")]
        public int? QtdCaixas { get; set; }
    }
}
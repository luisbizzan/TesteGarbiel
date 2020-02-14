using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.EtiquetaCtx
{
    public class LoteEtiquetaViewModel
    {
        public int? IdImpressora { get; set; }

        [Display(Name = "Tipo Etiqueta")]
        public int TipoEtiquetagem { get; set; }

        [Display(Name = "Lote")]
        public long? NroLote { get; set; }
      
        [Display(Name = "Produto")]
        public string DescricaoProduto { get; set; }

        [Display(Name = "Quantidade por Caixa")]
        public int? QtdPorCaixa { get; set; }

        [Display(Name = "Quantidade de Caixas")]
        public int? QtdCaixas { get; set; }
        [Required]
        public long IdProduto { get; set; }
    }
}
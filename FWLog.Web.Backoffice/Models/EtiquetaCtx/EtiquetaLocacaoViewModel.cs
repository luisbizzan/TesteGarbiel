using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.EtiquetaCtx
{
    public class LocacaoEtiquetaViewModel
    {
        public int? IdImpressora { get; set; }
        
        [Required]
        [Display(Name = "Ponto de Armazenagem")]
        public int IdPontoArmazenagem { get; set; }
        public string DescricaoPontoArmazenagem { get; set; }

        [Required]
        public int TipoEtiqueta { get; set; }

        [Required]
        public int TamanhoEtiqueta { get; set; }

        [Required]
        [Display(Name = "Corredor")]
        public int? Corredor { get; set; }
        [Display(Name = "De vertical nº")]
        public int? VerticalInicio { get; set; }
        [Display(Name = "Até vertical nº")]
        public int? VerticalFim { get; set; }
    }
}
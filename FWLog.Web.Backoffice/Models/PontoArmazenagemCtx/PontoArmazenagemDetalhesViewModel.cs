using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.PontoArmazenagemCtx
{
    public class PontoArmazenagemDetalhesViewModel
    {
        [Display(Name = "Código")]
        public long IdPontoArmazenagem { get; set; }
        [Display(Name = "Nível de Armazenagem")]
        public string NivelArmazenagem { get; set; }
        [Display(Name = "Ponto de Armazenagem")]
        public string PontoArmazenagem { get; set; }
        [Display(Name = "Tipo de Armazenagem")]
        public string TipoArmazenagem { get; set; }
        [Display(Name = "Tipo de Movimentação")]
        public string TipoMovimentacao { get; set; }
        [Display(Name = "Limite de Peso Vertical (KG)")]
        public decimal LimitePesoVertical { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.ArmazenagemCtx
{
    public class ImprimirRelatorioTotalPorAlaViewModel
    {
        public long IdNivelArmazenagem { get; set; }
        public long IdPontoArmazenagem { get; set; }
        public int? CorredorInicial { get; set; }
        public int? CorredorFinal { get; set; }
        public bool ImprimirVazia { get; set; }
        [Required]
        public int IdImpressora { get; set; }
    }
}
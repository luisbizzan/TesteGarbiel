using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class BOImprimirEtiquetaNotaRecebimentoViewModel
    {
        [Required]
        public long IdImpressora { get; set; }
        [Required]
        public long IdNotaFiscalRecebimento { get; set; }
    }
}
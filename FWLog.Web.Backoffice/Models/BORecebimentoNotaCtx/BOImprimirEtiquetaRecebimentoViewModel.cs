using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class BOImprimirEtiquetaRecebimentoViewModel
    {
        [Required]
        public long IdImpressora { get; set; }
        [Required]
        public long IdNotaFiscal { get; set; }
    }
}
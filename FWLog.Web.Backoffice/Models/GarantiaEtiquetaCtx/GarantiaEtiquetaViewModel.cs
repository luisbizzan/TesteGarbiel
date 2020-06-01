using FWLog.Data.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.GarantiaEtiquetaCtx
{
    public class GarantiaEtiquetaViewModel
    {
        [Required]
        public string Impressora { get; set; }
        [Required]
        public List<string> EtiquetaImpressaoIds { get; set; }
    }
}
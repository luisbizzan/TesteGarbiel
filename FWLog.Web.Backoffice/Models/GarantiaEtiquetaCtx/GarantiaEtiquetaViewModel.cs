using FWLog.Data.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.GarantiaEtiquetaCtx
{
    public class GarantiaEtiquetaViewModel
    {
        [Required]
        public int IdPerfilImpressora { get; set; }
        [Required]
        public int IdEmpresa { get; set; }
        [Required]
        public List<string> EtiquetaImpressaoIds { get; set; }
    }
}
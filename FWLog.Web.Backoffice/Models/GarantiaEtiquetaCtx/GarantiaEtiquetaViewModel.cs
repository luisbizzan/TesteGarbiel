using FWLog.Data.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.GarantiaEtiquetaCtx
{
    public class GarantiaEtiquetaViewModel
    {
        [Required]
        [Display(Name = "Tipo Etiqueta")]
        public GarantiaEtiqueta.ETIQUETA TipoEtiqueta { get; set; }

        [Required]
        [Display(Name = "Id Etiqueta")]
        public string IdEtiqueta { get; set; }
    }
}
using FWLog.Data.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.GarantiaEtiquetaCtx
{
    public class GarantiaEtiquetaViewModel
    {

        [Required]
        public List<int> EtiquetaImpressaoIds { get; set; }

        //#region Propriedades
        //public class Etiqueta
        //{
        //    [Display(Name = "Tipo Etiqueta")]
        //    public GarantiaEtiqueta.ETIQUETA TipoEtiqueta { get; set; }

        //    [Display(Name = "Id Etiqueta")]
        //    public string IdEtiqueta { get; set; }
        //}
        //#endregion
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.GeralCtx
{
    public class GeralHistoricoViewModel
    {
        [Required]
        public long Id_Categoria { get; set; }

        [Required]
        public long Id_Ref { get; set; }

        public string Id_Usr { get; set; }

        [Required]
        public string Historico { get; set; }

        public string Usuario { get; set; }

        public DateTime Dt_Cad { get; set; }

        public List<GeralHistoricoViewModel> Lista_Historicos { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.GarantiaCtx
{
    public class GarantiaLaudoVM
    {
        public GarantiaLaudo Form { get; set; }
        public List<GarantiaLaudo> Lista { get; set; }

        public GarantiaLaudoVM()
        {
            Form = new GarantiaLaudo();
            Lista = new List<GarantiaLaudo>();
        }
    }

    public class GarantiaLaudo
    {
        public SelectList Lista_Motivos { get; set; }

        [Display(Name = "Motivo")]
        public long Id_Motivo { get; set; }

        public string Motivo { get; set; }

        [Display(Name = "Referência")]
        public string Refx { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Quant.")]
        public long Quant { get; set; }
    }
}
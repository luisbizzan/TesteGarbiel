using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.GarantiaCtx
{
    public class GarantiaLaudoVM
    {
        public GarantiaLaudo Form { get; set; }
        public GarantiaConferenciaItem Conferencia { get; set; }
        public List<GarantiaLaudo> Lista { get; set; }

        public GarantiaLaudoVM()
        {
            Form = new GarantiaLaudo();
            Conferencia = new GarantiaConferenciaItem();
            Lista = new List<GarantiaLaudo>();
        }
    }

    public class GarantiaLaudo
    {
        public long Id { get; set; }

        [Display(Name = "Item NF")]
        public long Id_Item_Nf { get; set; }

        public string Item_Nf { get; set; }

        [Display(Name = "Item NF")]
        public long Id_Item { get; set; }

        public string Item { get; set; }
        public SelectList Lista_Motivos { get; set; }

        [Display(Name = "Motivo")]
        public long Id_Motivo { get; set; }

        [Display(Name = "Solicitação")]
        public long Id_Solicitacao { get; set; }

        public string Motivo { get; set; }

        [Display(Name = "Referência")]
        public string Refx { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Quant.")]
        public long Quant { get; set; }

        public long Tem_No_Excesso { get; set; }
        public long Quant_Laudo { get; set; }
        public long Quant_Max { get; set; }
    }
}
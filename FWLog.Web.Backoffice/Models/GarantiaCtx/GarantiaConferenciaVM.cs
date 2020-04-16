using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FWLog.Web.Backoffice.Models.GarantiaCtx
{
    public class GarantiaConferenciaVM
    {
        public GarantiaSolicitacaoListVM Solicitacao { get; set; }
        public GarantiaConferencia Form { get; set; }

        public GarantiaConferenciaVM()
        {
            Solicitacao = new GarantiaSolicitacaoListVM();
            Form = new GarantiaConferencia();
        }
    }

    public class GarantiaConferencia
    {
        [Display(Name = "Refx")]
        public string Refx { get; set; }

        [Display(Name = "Quantidade")]
        public long? Quantidade { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
    }
}
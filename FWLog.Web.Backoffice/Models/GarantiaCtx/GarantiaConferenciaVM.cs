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

        public GarantiaConferenciaVM()
        {
            Solicitacao = new GarantiaSolicitacaoListVM();
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

    public class GarantiaConferenciaDivergenciaVM
    {
        public GarantiaConferenciaDivergencia Cabecalho { get; set; }
        public List<GarantiaConferenciaDivergencia> Itens { get; set; }

        public GarantiaConferenciaDivergenciaVM()
        {
            Cabecalho = new GarantiaConferenciaDivergencia();
            Itens = new List<GarantiaConferenciaDivergencia>();
        }
    }

    public class GarantiaConferenciaDivergencia
    {
        [Display(Name = "Refx")]
        public string Refx { get; set; }

        [Display(Name = "Quant.")]
        public long? Quant { get; set; }

        [Display(Name = "Quant. Conf.")]
        public long? Quant_Conferida { get; set; }

        [Display(Name = "Divergência")]
        public long? Divergencia { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
    }

    public class GarantiaConferenciaFormVM
    {
        public GarantiaConferencia Form { get; set; }
        public List<GarantiaSolicitacaoItemListVM> Historicos { get; set; }

        public GarantiaConferenciaFormVM()
        {
            Historicos = new List<GarantiaSolicitacaoItemListVM>();
            Form = new GarantiaConferencia();
        }
    }
}
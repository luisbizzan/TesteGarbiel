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
        public GarantiaConferencia Conferencia { get; set; }

        public GarantiaConferenciaVM()
        {
            Solicitacao = new GarantiaSolicitacaoListVM();
            Conferencia = new GarantiaConferencia();
        }
    }

    public class GarantiaConferencia
    {
        public long Id_Tipo_Conf { get; set; }
        public String Tipo_Conf { get; set; }
        public DateTime Dt_Conf { get; set; }
        public long Id_Remessa { get; set; }
        public long Id_Solicitacao { get; set; }

        public long? Id { get; set; }
    }

    public class GarantiaConferenciaItem
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

        public long Id_Conf { get; set; }
        public long? Id { get; set; }
    }

    public class GarantiaConferenciaDivergenciaVM
    {
        public GarantiaConferenciaItem Cabecalho { get; set; }
        public List<GarantiaConferenciaItem> Itens { get; set; }

        public GarantiaConferenciaDivergenciaVM()
        {
            Cabecalho = new GarantiaConferenciaItem();
            Itens = new List<GarantiaConferenciaItem>();
        }
    }

    public class GarantiaConferenciaFormVM
    {
        public GarantiaConferenciaItem Form { get; set; }
        public List<GarantiaSolicitacaoItemListVM> Historicos { get; set; }

        public GarantiaConferenciaFormVM()
        {
            Historicos = new List<GarantiaSolicitacaoItemListVM>();
            Form = new GarantiaConferenciaItem();
        }
    }
}
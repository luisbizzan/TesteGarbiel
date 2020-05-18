using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.GarantiaCtx
{
    public class GarantiaConferenciaVM
    {
        public GarantiaSolicitacaoListVM Solicitacao { get; set; }
        public GarantiaConferencia Conferencia { get; set; }

        public GarantiaRemessaListVM Remessa { get; set; }

        public GarantiaConferenciaVM()
        {
            Solicitacao = new GarantiaSolicitacaoListVM();
            Conferencia = new GarantiaConferencia();
            Remessa = new GarantiaRemessaListVM();
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
        public long? Quant_Max { get; set; }

        [Display(Name = "Refx")]
        public string Refx { get; set; }

        [Display(Name = "Usuário")]
        public string Usr { get; set; }

        [Display(Name = "Quant. NF.")]
        public long? Quant { get; set; }

        [Display(Name = "Solicitação")]
        public long? Id_Solicitacao { get; set; }

        public long Id_Tipo_Conf { get; set; }

        public DateTime Dt_Conf { get; set; }

        [Display(Name = "Quant. Conf.")]
        public long? Quant_Conferida { get; set; }

        [Display(Name = "Divergência")]
        public long? Divergencia { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        public long Id_Conf { get; set; }
        public long? Id { get; set; }
        public long Id_Item { get; set; }

        public long Tem_No_Excesso { get; set; }
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

        public SelectList Lista_Refx { get; set; }
        public SelectList Lista_Solicitacao { get; set; }

        public GarantiaConferenciaFormVM()
        {
            Historicos = new List<GarantiaSolicitacaoItemListVM>();
            Form = new GarantiaConferenciaItem();
        }
    }
}
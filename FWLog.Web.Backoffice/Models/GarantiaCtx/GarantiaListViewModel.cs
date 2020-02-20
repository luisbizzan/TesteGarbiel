using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.GarantiaCtx
{
    public class GarantiaListViewModel
    {
        public GarantiaListItemViewModel EmptyItem { get; set; }

        public GarantiaFilterViewModel Filter { get; set; }

        public GarantiaListViewModel()
        {
            EmptyItem = new GarantiaListItemViewModel();
            Filter = new GarantiaFilterViewModel();
        }
    }

    public class GarantiaListItemViewModel
    {
        public long IdEmpresa { get; set; }
        public long? IdGarantia { get; set; }
        [Display(Name = "Cliente")]
        public long? IdCliente { get; set; }
        public long? IdTransportadora { get; set; }
        public long? NumeroNF { get; set; }
        public string NumeroFicticioNF { get; set; }
        public long? ChaveAcesso { get; set; }
        public DateTime? DataEmissao{ get; set; }        
        public DateTime? DataRecebimento { get; set; }        
        public long? IdTranpostadora { get; set; }
        public string IdUsuarioRecebimento { get; set; }
        public string IddUsuarioConferente { get; set; }
        public long? IdGarantiaStatus { get; set; }
    }

    public class GarantiaFilterViewModel
    {
        public long IdEmpresa { get; set; }
        [Display(Name = "Nro Solic. Garantia")]
        public long? IdGarantia { get; set; }
        [Display(Name = "Cliente")]
        public long? IdCliente { get; set; }
        [Display(Name = "Transportadora")]
        public long? IdTransportadora { get; set; }
        [Display(Name = "Nota Fiscal")]
        public long? NumeroNF { get; set; }
        [Display(Name = "Número Fictício NF")]
        public string NumeroFicticioNF { get; set; }
        [Display(Name = "Chave de Acesso")]
        public string RazaoSocialCliente { get; set; }
        public string RazaoSocialTransportadora { get; set; }
        public string ChaveAcesso { get; set; }
        [Display(Name = "Data Emissão Inicial")]
        public DateTime? DataEmissaoInicial { get; set; }
        [Display(Name = "Data Emissão Final")]
        public DateTime? DataEmissaoFinal { get; set; }
        [Display(Name = "Data Recebimento Inicial")]
        public DateTime? DataRecebimentoInicial { get; set; }
        [Display(Name = "Data Recebimento Final")]
        public DateTime? DataRecebimentoFinal { get; set; }
        [Display(Name = "Recebido por")]
        public string IdUsuarioRecebimento { get; set; }        
        public string UserNameRecebimento { get; set; }
        [Display(Name = "Status")]
        public long? IdGarantiaStatus { get; set; }
        [Display(Name = "Conferido Por")]
        public string IdUsuarioConferencia { get; set; }
        public string UserNameConferencia { get; set; }
        public SelectList ListaStatus { get; set; }
    }
}
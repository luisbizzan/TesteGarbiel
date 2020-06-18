using System;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.ExpedicaoCtx
{
    public class RelatorioPedidodsExpedidosViewModel
    {
        public RelatorioPedidosExpedidosListItemViewModel EmptyItem { get; set; }
        public RelatorioPedidosExpedidosFilterViewModel Filter { get; set; }

        public RelatorioPedidodsExpedidosViewModel()
        {
            EmptyItem = new RelatorioPedidosExpedidosListItemViewModel();
            Filter = new RelatorioPedidosExpedidosFilterViewModel();
        }
    }

    public class RelatorioPedidosExpedidosListItemViewModel
    {
        [Display(Name = "Nro. Pedido")]
        public long NroPedido { get; set; }

        [Display(Name = "Volume")]
        public string NroVolume { get; set; }

        [Display(Name = "Centena")]
        public string NroCentena { get; set; }

        [Display(Name = "Data do Pedido")]
        public string DataDoPedido { get; set; }

        [Display(Name = "Data Integração")]
        public string DataIntegracaoPedido { get; set; }

        [Display(Name = "Transportadora")]
        public string IdTransportadora { get; set; }

        [Display(Name = "Nota Fiscal")]
        public string NotaFiscalESerie { get; set; }

        [Display(Name = "Data Saída do Pedido")]
        public string DataSaidaDoPedido { get; set; }
    }

    public class RelatorioPedidosExpedidosFilterViewModel
    {
        [Display(Name = "Data Inícial")]
        [Required]
        public DateTime? DataInicial { get; set; }

        [Display(Name = "Data Final")]
        [Required]
        public DateTime? DataFinal { get; set; }

        [Display(Name = "Transportadora")]
        public long? IdTransportadora { get; set; }

        public string NomeTransportadora { get; set; }
    }
}
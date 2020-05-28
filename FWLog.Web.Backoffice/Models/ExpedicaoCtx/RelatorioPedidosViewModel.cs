using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace FWLog.Web.Backoffice.Models.ExpedicaoCtx
{
    public class RelatorioPedidosViewModel
    {
        public RelatorioPedidosListItemViewModel EmptyItem { get; set; }
        public RelatorioPedidosFilterViewModel Filter { get; set; }

        public RelatorioPedidosViewModel()
        {
            EmptyItem = new RelatorioPedidosListItemViewModel();
            Filter = new RelatorioPedidosFilterViewModel();
        }
    }

    public class RelatorioPedidosListItemViewModel
    {
        public long IdPedidoVendaVolume { get; set; }

        [Display(Name = "Nro. Pedido")]
        public long NroPedido { get; set; }

        [Display(Name = "Nro. Volume")]
        public string NroVolume { get; set; }

        [Display(Name = "Transportadora")]
        public string NomeTransportadora { get; set; }
 
        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Data Criação")]
        public DateTime DataDoPedido { get; set; }

        [Display(Name = "Data Saída")]
        public DateTime? DataSaidaDoPedido { get; set; }
    }

    public class RelatorioPedidosFilterViewModel
    {
        [Display(Name = "Nro. Pedido")]
        public long? IdPedidoVenda { get; set; }

        public string NumeroPedidoVenda { get; set; }

        [Display(Name="Data Inícial")]
        [Required]
        public DateTime? DataInicial { get; set; }

        [Display(Name = "Data Final")]
        [Required]
        public DateTime? DataFinal { get; set; }

        [Display(Name = "Transportadora")]
        public long? IdTransportadora { get; set; }

        public string NomeTransportadora { get; set; }

        [Display(Name = "Nro. Pedido")]
        public int NroVolume { get; set; }

        public SelectList ListaStatus { get; set; }
        [Display(Name = "Status")]
        public long? IdStatus { get; set; }

    }
}
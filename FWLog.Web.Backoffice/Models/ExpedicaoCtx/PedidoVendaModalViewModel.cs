using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.ExpedicaoCtx
{
    public class PedidoVendaModalViewModelViewModel
    {
        public PedidoVendaModalViewModelViewModel()
        {
            EmptyItem = new PedidoVendaModalViewModelListItemViewModel();
            Filter = new PedidoVendaModalViewModelFilterViewModel();
        }

        public PedidoVendaModalViewModelListItemViewModel EmptyItem { get; set; }
        public PedidoVendaModalViewModelFilterViewModel Filter { get; set; }
    }

    public class PedidoVendaModalViewModelListItemViewModel
    {
        public long IdPedidoVenda { get; set; }

        [Display(Name = "Pedido")]
        public long NumeroPedido { get; set; }

        [Display(Name = "Pedido Venda")]
        public int NumeroPedidoVenda { get; set; }

        [Display(Name = "Cliente")]
        public string ClienteNome { get; set; }

        [Display(Name = "Transportadora")]
        public string TransportadoraNome { get; set; }
    }

    public class PedidoVendaModalViewModelFilterViewModel
    {
        [Display(Name = "Número Pedido")]
        public string NumeroPedido { get; set; }

        [Display(Name = "Número Pedido Venda")]
        public string NumeroPedidoVenda { get; set; }

        [Display(Name = "Transportadora")]
        public string NomeTransportadora { get; set; }
    }
}
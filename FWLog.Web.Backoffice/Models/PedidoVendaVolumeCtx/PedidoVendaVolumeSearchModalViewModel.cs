using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.PedidoVendaVolumeCtx
{
    public class PedidoVendaVolumeSearchModalViewModel
    {
        public PedidoVendaVolumeSearchModalItemViewModel EmptyItem { get; set; }
        public PedidoVendaVolumeSearchModalFillterViewModel Filter { get; set; }

        public PedidoVendaVolumeSearchModalViewModel()
        {
            EmptyItem = new PedidoVendaVolumeSearchModalItemViewModel();
            Filter = new PedidoVendaVolumeSearchModalFillterViewModel();
        }
    }

    public class PedidoVendaVolumeSearchModalItemViewModel
    {
        public long IdPedidoVendaVolume { get; set; }

        [Display(Name = "Nro. Pedido")]
        public long NroPedido { get; set; }

        [Display(Name = "Nro. Volume")]
        public string NroVolume { get; set; }

        [Display(Name = "Status")]
        public string DescricaoStatus { get; set; }
    }

    public class PedidoVendaVolumeSearchModalFillterViewModel
    {
        public long IdPedidoVendaVolume { get; set; }

        [Display(Name = "Nro. Pedido")]
        public long NroPedido { get; set; }

        [Display(Name = "Nro. Volume")]
        public string NroVolume { get; set; }
    }
}
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

        [Display(Name = "Pedido")]
        public long IdPedido { get; set; }

        [Display(Name = "Nro. Volume")]
        public string NroVolume { get; set; }
    }

    public class PedidoVendaVolumeSearchModalFillterViewModel
    {
        [Display(Name = "Código")]
        public long? IdCliente { get; set; }

        [Display(Name = "Razão Social")]
        public string RazaoSocial { get; set; }

        [Display(Name = "CNPJ/CPF")]
        public string CNPJCPF { get; set; }
    }
}
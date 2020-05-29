using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.ExpedicaoCtx
{
    public class ConfirmaReimpressaoVolumeViewModel
    {
        public long IdPedidoVendaVolume { get; set; }

        [Display(Name = "Pedido")]
        public int NroPedido { get; set; }

        [Display(Name = "Volume")]
        public int Volume { get; set; }
    }
}
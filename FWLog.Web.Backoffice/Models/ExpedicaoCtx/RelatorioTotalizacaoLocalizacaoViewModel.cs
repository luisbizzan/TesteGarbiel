using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.ExpedicaoCtx
{
    public class RelatorioVolumesInstaladosTransportadoraViewModel
    {
        public RelatorioVolumesInstaladosTransportadoraViewModel()
        {
            EmptyItem = new RelatorioVolumesInstaladosTransportadoraListItemViewModel();
            Filter = new RelatorioVolumesInstaladosTransportadoraFilterViewModel();
        }

        public RelatorioVolumesInstaladosTransportadoraListItemViewModel EmptyItem { get; set; }
        public RelatorioVolumesInstaladosTransportadoraFilterViewModel Filter { get; set; }
    }

    public class RelatorioVolumesInstaladosTransportadoraListItemViewModel
    {
        [Display(Name = "Transportadora")]
        public string Transportadora { get; set; }

        [Display(Name = "Endereço")]
        public string CodigoEndereco { get; set; }

        [Display(Name = "Pedido")]
        public string NumeroPedido { get; set; }

        [Display(Name = "Volume")]
        public string NumeroVolume { get; set; }
    }

    public class RelatorioVolumesInstaladosTransportadoraFilterViewModel
    {
        [Display(Name = "Transportadora")]
        public long? IdTransportadora { get; set; }

        public string NomeTransportadora { get; set; }

        [Display(Name = "Pedido")]
        public long? IdPedidoVenda { get; set; }

        public string NumeroPedidoVenda { get; set; }
    }
}
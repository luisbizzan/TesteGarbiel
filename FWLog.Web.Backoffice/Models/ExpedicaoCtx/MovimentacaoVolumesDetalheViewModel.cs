using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.ExpedicaoCtx
{
    public class MovimentacaoVolumesDetalheViewModel
    {
        [Display(Name = "Status")]
        public string Status { get; set; }

        public List<MovimentacaoVolumesDetalheListItemViewModel> Items { get; set; }

        public string Url { get; set; }
    }

    public class MovimentacaoVolumesDetalheListItemViewModel
    {
        [Display(Name = "Pedido")]
        public string PedidoNumero { get; set; }

        [Display(Name = "Volume")]
        public int VolumeNumero { get; set; }

        [Display(Name = "Quantidade")]
        public int QuantidadeProdutos { get; set; }

        [Display(Name = "Data Pedido")]
        public DateTime PedidoData { get; set; }

        public long IdPedidoVendaVolume { get; set; }
    }
}
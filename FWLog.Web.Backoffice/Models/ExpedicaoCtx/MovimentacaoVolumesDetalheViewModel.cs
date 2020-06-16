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
        public long IdPedidoVendaVolume { get; set; }

        [Display(Name = "Pedido")]
        public string PedidoNumero { get; set; }

        [Display(Name = "Volume")]
        public int VolumeNumero { get; set; }

        [Display(Name = "Quantidade")]
        public int QuantidadeProdutos { get; set; }

        [Display(Name = "Data Criação")]
        public DateTime PedidoData { get; set; }

        [Display(Name = "Centena")]
        public int VolumeCentena { get; set; }

        [Display(Name = "Transportadora")]
        public string TransportadoraNomeFantasia { get; set; }

        [Display(Name = "Tipo Pagamento")]
        public string TipoPagamentoDescricao { get; set; }

        [Display(Name = "Usuário Despacho NF")]
        public string UsuarioDespachoNotaFiscal { get; set; }

        [Display(Name = "Data Despacho NF")]
        public DateTime? DataHoraDespachoNotaFiscal { get; set; }

        [Display(Name = "Número Romaneio")]
        public int? NumeroRomaneio { get; set; }

        [Display(Name = "Usuário Romaneio")]
        public string UsuarioRomaneio { get; set; }

        [Display(Name = "Data Romaneio")]
        public DateTime? DataHoraRomaneio { get; set; }
    }
}
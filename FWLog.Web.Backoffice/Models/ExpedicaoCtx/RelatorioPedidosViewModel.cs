using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

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
        public bool PermitirEditarVolume { get; set; }

        [Display(Name = "Nro. Pedido")]
        public string NroPedido { get; set; }

        [Display(Name = "Nro. Volume")]
        public string NroVolume { get; set; }

        [Display(Name = "Centena")]
        public string NroCentena { get; set; }

        [Display(Name = "Transportadora")]
        public string NomeTransportadora { get; set; }

        [Display(Name = "Status Volume")]
        public string StatusVolume { get; set; }

        [Display(Name = "Status Pedido")]
        public string StatusPedido { get; set; }

        [Display(Name = "Data Criação")]
        public string DataCriacao { get; set; }

        [Display(Name = "Data Integração")]
        public string DataIntegracao { get; set; }

        [Display(Name = "Data Impressão")]
        public string DataImpressao { get; set; }

        [Display(Name = "Número/Série NF")]
        public string NumeroSerieNotaFiscal { get; set; }

        [Display(Name = "Data Expedição")]
        public string DataExpedicao { get; set; }

        public bool PodeRemoverUsuarioSeparacao { get; set; }
    }

    public class RelatorioPedidosFilterViewModel
    {
        [Display(Name = "Nro. Pedido")]
        public string NumeroPedido { get; set; }

        [Display(Name = "Dt. Impressão Inícial")]
        public DateTime? DataInicial { get; set; }

        [Display(Name = "Dt. Impressão Final")]
        public DateTime? DataFinal { get; set; }

        [Display(Name = "Transportadora")]
        public long? IdTransportadora { get; set; }

        public string NomeTransportadora { get; set; }

        [Display(Name = "Cliente")]
        public long? IdCliente { get; set; }

        public string NomeCliente { get; set; }

        public SelectList ListaStatus { get; set; }

        [Display(Name = "Status")]
        public long? IdStatus { get; set; }

        [Display(Name = "Produto")]
        public long? IdProduto { get; set; }
        public string DescricaoProduto { get; set; }
    }
}
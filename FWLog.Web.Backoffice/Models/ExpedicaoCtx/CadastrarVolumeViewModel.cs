using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.ExpedicaoCtx
{
    public class CadastrarVolumeViewModel
    {
        public long IdEmpresa { get; set; }

        [Display(Name = "Nro. Pedido")]
        [Required]
        public long IdPedido { get; set; }

        public long IdProduto { get; set; }

        [Display(Name = "Produto")]
        public string DescricaoProduto { get; set; }

        [Display(Name = "Nro Volume")]
        public string Volume { get; set; }

        [Display(Name = "Qtd. Original")]
        public int QuantidadeOriginal { get; set; }

        [Display(Name = "Qtd. Transferir")]
        public int Quantidade { get; set; }

        public List<CadastrarVolumeItemViewModel> ListaItens { get; set; } = new List<CadastrarVolumeItemViewModel>();
    }

    public class CadastrarVolumeItemViewModel
    {
        public long IdProduto { get; set; }

        public long IdPedidoVendaVolumeOrigem { get; set; }

        public long? IdPedidoVendaVolume { get; set; }

        public int QuantidadeOriginal { get; set; }

        public int Quantidade { get; set; }
    }
}
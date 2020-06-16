using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.ExpedicaoCtx
{
    public class GerenciarVolumeViewModel
    {
        public long IdEmpresa { get; set; }

        [Display(Name = "Nro. Pedido")]
        [Required]
        public int NroPedido { get; set; }

        public long IdPedido { get; set; }

        public long IdProduto { get; set; }

        [Display(Name = "Produto")]
        public string DescricaoProduto { get; set; }

        [Display(Name = "Nro Volume")]
        public string NroVolume { get; set; }

        public long IdPedidoVendaVolumeOrigem { get; set; }

        public long? IdPedidoVendaVolume { get; set; }

        [Display(Name = "Qtd. Original")]
        public int QuantidadeOriginal { get; set; }

        [Display(Name = "Qtd. Transferir")]
        public int Quantidade { get; set; }
        public long IdGrupoCorredorArmazenagem { get; set; }
        public int CorredorInicio { get; set; }
        public int CorredorFim { get; set; }

        public List<GerenciarVolumeItemViewModel> ListaItens { get; set; } = new List<GerenciarVolumeItemViewModel>();
    }

    public class GerenciarVolumeItemViewModel
    {
        public long IdProduto { get; set; }

        public long IdPedidoVendaVolumeOrigem { get; set; }

        public int Quantidade { get; set; }
    }
}
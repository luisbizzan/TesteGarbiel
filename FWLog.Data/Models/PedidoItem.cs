using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class PedidoItem
    {
        [Key]
        [Required]
        public long IdPedidoItem { get; set; }

        [Required]
        [Index]
        public long IdPedido { get; set; }

        [Required]
        [Index]
        public long IdProduto { get; set; }

        [Required]
        public int QtdPedido { get; set; }

        [Required]
        public int Sequencia { get; set; }

        [ForeignKey(nameof(IdPedido))]
        public virtual Pedido Pedido { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public virtual Produto Produto { get; set; }
    }
}

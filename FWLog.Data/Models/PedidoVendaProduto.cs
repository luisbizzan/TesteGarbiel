using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class PedidoVendaProduto
    {
        [Key]
        [Required]
        public long IdPedidoVendaProduto { get; set; }

        [Required]
        [Index]
        public long IdPedidoVenda { get; set; }

        [Required]
        [Index]
        public long IdPedidoVendaVolume { get; set; }

        [Required]
        [Index]
        public long IdProduto { get; set; }

        [Required]
        public int QtdPedido { get; set; }

        [Required]
        public int Sequencia { get; set; }

        [Index]
        [Required]
        public PedidoVendaProdutoStatusEnum IdPedidoVendaProdutoStatus { get; set; }

        [Required]
        public decimal CubagemProduto { get; set; }

        [Required]
        public decimal PesoProdutoKg { get; set; }

        [Required]
        public long IdEnderecoArmazenagem { get; set; }

        public int? QtdSeparada { get; set; }

        public DateTime? DataHoraFimSeparacao { get; set; }

        public DateTime? DataHoraInicioSeparacao { get; set; }

        public string IdUsuarioAutorizacaoZerar { get; set; }

        [ForeignKey(nameof(IdPedidoVenda))]
        public virtual PedidoVenda PedidoVenda { get; set; }

        [ForeignKey(nameof(IdPedidoVendaVolume))]
        public virtual PedidoVendaVolume PedidoVendaVolume { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public virtual Produto Produto { get; set; }

        [ForeignKey(nameof(IdPedidoVendaProdutoStatus))]
        public virtual PedidoVendaProdutoStatus PedidoVendaProdutoStatus { get; set; }

        [ForeignKey(nameof(IdEnderecoArmazenagem))]
        public virtual EnderecoArmazenagem EnderecoArmazenagem { get; set; }
    }
}
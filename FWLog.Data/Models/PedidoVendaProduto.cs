﻿using System.ComponentModel.DataAnnotations;
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
        public long IdProduto { get; set; }

        [Required]
        public int QtdPedido { get; set; }

        [Index]
        [Required]
        public PedidoVendaProdutoStatusEnum IdPedidoVendaProdutoStatus { get; set; }

        [ForeignKey(nameof(IdPedidoVenda))]
        public virtual PedidoVenda PedidoVenda { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public virtual Produto Produto { get; set; }

        [ForeignKey(nameof(IdPedidoVendaProdutoStatus))]
        public virtual PedidoVendaProdutoStatus PedidoVendaProdutoStatus { get; set; }
    }
}

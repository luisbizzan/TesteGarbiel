using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class Pedido
    {
        public Pedido()
        {
            PedidoItens = new HashSet<PedidoItem>();
        }

        [Key]
        [Required]
        public long IdPedido { get; set; }

        [Index]
        [Required]
        public long IdEmpresa { get; set; }

        [Index]
        [Required]
        public long IdCliente { get; set; }

        [Index]
        [Required]
        public long IdTransportadora { get; set; }

        [Index]
        [Required]
        public long IdRepresentante { get; set; }

        [Required]
        public int NroPedido { get; set; }

        [Index]
        [Required]
        public PedidoStatusEnum IdPedidoStatus { get; set; }

        [Required]
        public int CodigoIntegracao { get; set; }

        [Required]
        public DateTime DataCriacao { get; set; }

        public virtual ICollection<PedidoItem> PedidoItens { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey(nameof(IdRepresentante))]
        public virtual Representante Representante { get; set; }

        [ForeignKey(nameof(IdCliente))]
        public virtual Cliente Cliente { get; set; }

        [ForeignKey(nameof(IdTransportadora))]
        public virtual Produto Transportadora { get; set; }

        [ForeignKey(nameof(IdPedidoStatus))]
        public virtual PedidoStatus PedidoVendaStatus { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class PedidoVenda
    {
        public PedidoVenda()
        {
            PedidoVendaProdutos = new HashSet<PedidoVendaProduto>();
            PedidoVendaVolumes = new HashSet<PedidoVendaVolume>();
        }

        [Key]
        [Required]
        public long IdPedidoVenda { get; set; }

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
        public int NroPedidoVenda { get; set; }

        public int NroVolumes { get; set; }

        [Index]
        [Required]
        public PedidoVendaStatusEnum IdPedidoVendaStatus { get; set; }

        [Index]
        public string IdUsuarioSeparacao { get; set; }

        public DateTime? DataHoraInicioSeparacao { get; set; }

        public DateTime? DataHoraFimSeparacao { get; set; }

        [Required]
        public DateTime DataCriacao { get; set; }

        public virtual ICollection<PedidoVendaProduto> PedidoVendaProdutos { get; set; }

        [ForeignKey(nameof(IdPedido))]
        public virtual Pedido Pedido { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey(nameof(IdRepresentante))]
        public virtual Representante Representante { get; set; }

        [ForeignKey(nameof(IdCliente))]
        public virtual Cliente Cliente { get; set; }

        [ForeignKey(nameof(IdTransportadora))]
        public virtual Produto Transportadora { get; set; }

        [ForeignKey(nameof(IdPedidoVendaStatus))]
        public virtual PedidoVendaStatus PedidoVendaStatus { get; set; }

        [ForeignKey(nameof(IdUsuarioSeparacao))]
        public virtual AspNetUsers UsuarioSeparacao { get; set; }

        public virtual ICollection<PedidoVendaVolume> PedidoVendaVolumes { get; set; }
    }
}
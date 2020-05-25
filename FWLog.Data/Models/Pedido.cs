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

        [Required]
        public int NroPedido { get; set; }

        [Index]
        [Required]
        public long IdEmpresa { get; set; }

        [Index]
        [Required]
        public long IdCliente { get; set; }

        [Index]
        [Required]
        public long IdTransportadora { get; set; }

        [Required]
        public int CodigoIntegracao { get; set; }

        [Index]
        [Required]
        public long IdRepresentante { get; set; }

        [Required]
        public DateTime DataCriacao { get; set; }

        [Required]
        public bool IsRequisicao { get; set; }

        [Index]
        public long? CodigoIntegracaoNotaFiscal { get; set; }

        [Index]
        [Required]
        public PedidoVendaStatusEnum IdPedidoVendaStatus { get; set; }

        public string CodigoIntegracaoTipoFrete { get; set; }

        public string ChaveAcessoNotaFiscal { get; set; }

        public int? PagamentoCodigoIntegracao { get; set; }

        public string PagamentoDescricaoIntegracao { get; set; }

        public bool PagamentoIsDebitoIntegracao { get; set; }

        public bool PagamentoIsCreditoIntegracao { get; set; }

        public virtual ICollection<PedidoItem> PedidoItens { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey(nameof(IdRepresentante))]
        public virtual Representante Representante { get; set; }

        [ForeignKey(nameof(IdCliente))]
        public virtual Cliente Cliente { get; set; }

        [ForeignKey(nameof(IdTransportadora))]
        public virtual Transportadora Transportadora { get; set; }

        [ForeignKey(nameof(IdPedidoVendaStatus))]
        public virtual PedidoVendaStatus PedidoVendaStatus { get; set; }
    }
}
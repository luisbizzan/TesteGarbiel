using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class PedidoVendaProduto
    {
        [Key]
        [Required]
        [Index]
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
        public int QtdSeparar { get; set; }

        [Required]
        public int Sequencia { get; set; }

        [Index]
        [Required]
        public PedidoVendaStatusEnum IdPedidoVendaStatus { get; set; }

        [Required]
        public decimal CubagemProduto { get; set; }

        [Required]
        public decimal PesoProdutoKg { get; set; }

        [Required]
        [Index]
        public long IdEnderecoArmazenagem { get; set; }

        public int? QtdSeparada { get; set; }

        public int? QtdeSeparar { get; set; }

        public DateTime? DataHoraFimSeparacao { get; set; }

        public DateTime? DataHoraInicioSeparacao { get; set; }

        public string IdUsuarioAutorizacaoZerar { get; set; }

        public DateTime? DataHoraAutorizacaoZerarPedido { get; set; }

        [Index]
        public long? IdLote { get; set; }

        [Index]
        public string IdUsuarioSeparacao { get; set; }

        [ForeignKey(nameof(IdPedidoVenda))]
        public virtual PedidoVenda PedidoVenda { get; set; }

        [ForeignKey(nameof(IdPedidoVendaVolume))]
        public virtual PedidoVendaVolume PedidoVendaVolume { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public virtual Produto Produto { get; set; }

        [ForeignKey(nameof(IdPedidoVendaStatus))]
        public virtual PedidoVendaStatus PedidoVendaProdutoStatus { get; set; }

        [ForeignKey(nameof(IdEnderecoArmazenagem))]
        public virtual EnderecoArmazenagem EnderecoArmazenagem { get; set; }

        [ForeignKey(nameof(IdUsuarioSeparacao))]
        public virtual AspNetUsers UsuarioSeparacao { get; set; }

        [ForeignKey(nameof(IdLote))]
        public virtual Lote Lote { get; set; }
    }
}
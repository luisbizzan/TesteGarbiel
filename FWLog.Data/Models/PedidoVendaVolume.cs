using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class PedidoVendaVolume
    {
        public PedidoVendaVolume()
        {
            PedidoVendaProdutos = new HashSet<PedidoVendaProduto>();
        }

        [Key]
        [Required]
        public long IdPedidoVendaVolume { get; set; }

        [Required]
        [Index]
        public long IdPedidoVenda { get; set; }

        [Required]
        [Index]
        public int NroVolume { get; set; }

        [Required]
        [Index]
        public int NroCentena { get; set; }

        [Required]
        [StringLength(100)]
        [Index]
        public string EtiquetaVolume { get; set; }

        [Required]
        [Index]
        public long IdCaixaCubagem { get; set; }

        [Required]
        [Index]
        public decimal CubagemVolume { get; set; }

        [Required]
        [Index]
        public decimal PesoVolume { get; set; }

        [Required]
        [Index]
        public long IdGrupoCorredorArmazenagem { get; set; }

        [Required]
        [Index]
        public int CorredorInicio { get; set; }

        [Required]
        [Index]
        public int CorredorFim { get; set; }

        [Required]
        [Index]
        public long IdImpressora { get; set; }

        [Index]
        public long? IdCaixaVolume { get; set; }

        [Index]
        public DateTime? DataHoraInicioSeparacao { get; set; }

        [Index]
        public DateTime? DataHoraFimSeparacao { get; set; }

        [Index]
        [Required]
        public PedidoVendaStatusEnum IdPedidoVendaStatus { get; set; }

        [Index]
        public string IdUsuarioInstalTransportadora { get; set; }

        public DateTime? DataHoraInstalTransportadora { get; set; }

        [Index]
        public long? IdEnderecoArmazTransportadora { get; set; }

        [ForeignKey(nameof(IdPedidoVenda))]
        public virtual PedidoVenda PedidoVenda { get; set; }

        [ForeignKey(nameof(IdPedidoVendaStatus))]
        public virtual PedidoVendaStatus PedidoVendaProdutoStatus { get; set; }

        [ForeignKey(nameof(IdCaixaCubagem))]
        public virtual Caixa CaixaCubagem { get; set; }

        [ForeignKey(nameof(IdGrupoCorredorArmazenagem))]
        public virtual GrupoCorredorArmazenagem GrupoCorredorArmazenagem { get; set; }

        [ForeignKey(nameof(IdImpressora))]
        public virtual Printer Printer { get; set; }

        [ForeignKey(nameof(IdCaixaVolume))]
        public virtual Caixa CaixaVolume { get; set; }

        [ForeignKey(nameof(IdUsuarioInstalTransportadora))]
        public virtual AspNetUsers UsuarioInstalacaoTransportadora { get; set; }

        [ForeignKey(nameof(IdEnderecoArmazTransportadora))]
        public virtual EnderecoArmazenagem EnderecoTransportadora { get; set; }

        public virtual ICollection<PedidoVendaProduto> PedidoVendaProdutos { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class PedidoVendaVolume
    {
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
        public decimal PesoVolumeKG { get; set; }

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
        public long IdPerfilImpressora { get; set; }

        [Required]
        [Index]
        public long IdCaixaVolume { get; set; }

        [Index]
        public DateTime DataHoraInicioSeparacao { get; set; }

        [Index]
        public DateTime DataHoraFimSeparacao { get; set; }

        [Index]
        public string IdUsuarioSeparacao { get; set; }


        [Index]
        [Required]
        public PedidoVendaProdutoStatusEnum IdPedidoVendaProdutoStatus { get; set; }

        [ForeignKey(nameof(IdPedidoVenda))]
        public virtual PedidoVenda PedidoVenda { get; set; }

        [ForeignKey(nameof(IdPedidoVendaProdutoStatus))]
        public virtual PedidoVendaProdutoStatus PedidoVendaProdutoStatus { get; set; }

        [ForeignKey(nameof(IdCaixaCubagem))]
        public virtual Caixa CaixaCubagem { get; set; }

        //[ForeignKey(nameof(IdGrupoCorredorArmazenagem))]
        //public virtual GrupoCorredorArmazenagem GrupoCorredorArmazenagem { get; set; }

        [ForeignKey(nameof(IdPerfilImpressora))]
        public virtual PerfilImpressora PerfilImpressora { get; set; }

        [ForeignKey(nameof(IdCaixaVolume))]
        public virtual Caixa CaixaVolume { get; set; }

        [ForeignKey(nameof(IdUsuarioSeparacao))]
        public virtual AspNetUsers UsuarioSeparacao { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class RomaneioNotaFiscal
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdRomaneioNotaFiscal { get; set; }

        [Index]
        [Required]
        public long IdRomaneio { get; set; }

        [Index]
        [Required]
        public long IdPedidoVenda { get; set; }

        [Index]
        [Required]
        public int NroNotaFiscal { get; set; }

        [Index]
        [Required]
        public long IdCliente { get; set; }

        [Required]
        public int NroVolumes { get; set; }

        public decimal TotalPesoLiquidoVolumes { get; set; }

        public decimal TotalPesoBrutoVolumes { get; set; }

        [Index]
        [Required]
        [StringLength(3)]
        public string SerieNotaFiscal { get; set; }

        [ForeignKey(nameof(IdRomaneio))]
        public virtual Romaneio Romaneio { get; set; }

        [ForeignKey(nameof(IdPedidoVenda))]
        public virtual PedidoVenda PedidoVenda { get; set; }

        [ForeignKey(nameof(IdCliente))]
        public virtual Cliente Cliente { get; set; }
    }
}

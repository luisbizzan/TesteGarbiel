using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class LoteProduto
    {
        [Key]
        [Required]
        public long IdLoteProduto { get; set; }

        [Index]
        [Required]
        public long IdEmpresa { get; set; }

        [Index]
        [Required]
        public long IdLote { get; set; }

        [Index]
        [Required]
        public long IdProduto { get; set; }

        [Required]
        public long QuantidadeRecebida { get; set; }

        [Required]
        public long Saldo { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey(nameof(IdLote))]
        public virtual Lote Lote { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public virtual Produto Produto { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class CaixaRecusa
    {
        [Key]
        [Column(Order = 0)]
        [Required]
        public long IdEmpresa { get; set; }

        [Key]
        [Column(Order = 1)]
        [Required]
        public long IdCaixa { get; set; }

        [Key]
        [Column(Order = 2)]
        [Required]
        public long IdProduto { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey(nameof(IdCaixa))]
        public virtual Caixa Caixa { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public virtual Produto Produto { get; set; }
    }
}

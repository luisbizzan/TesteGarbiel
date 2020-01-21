using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class ProdutoEmpresa
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdProdutoEmpresa { get; set; }

        [Required]
        [Index]
        public long IdEmpresa { get; set; }

        [Required]
        [Index]
        public long IdProduto { get; set; }

        [Required]
        public bool Ativo { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public virtual Lote Produto { get; set; }
    }
}

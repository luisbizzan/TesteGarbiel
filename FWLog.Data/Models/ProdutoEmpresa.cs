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
        public long IdProduto { get; set; }

        [Required]
        [Index]
        public long IdEmpresa { get; set; }

        [Required]
        public int Saldo { get; set; }

        [Index]
        public long? IdEnderecoArmazenagem { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public virtual Produto Produto { get; set; }

        [ForeignKey(nameof(IdEnderecoArmazenagem))]
        public virtual EnderecoArmazenagem EnderecoArmazenagem { get; set; }
    }
}

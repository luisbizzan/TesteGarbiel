using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class ProdutoEstoque
    {
        [Key, Column(Order = 0)]
        [Required]
        public long IdProduto { get; set; }

        [Key, Column(Order = 1)]
        [Required]
        public long IdEmpresa { get; set; }

        [Required]
        public int Saldo { get; set; }

        [Index]
        public long? IdEnderecoArmazenagem { get; set; }

        public int DiasPrazoEntrega { get; set; }

        [Required]
        public ProdutoEstoqueStatusEnum IdProdutoEstoqueStatus { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public virtual Produto Produto { get; set; }

        [ForeignKey(nameof(IdEnderecoArmazenagem))]
        public virtual EnderecoArmazenagem EnderecoArmazenagem { get; set; }
    }
}

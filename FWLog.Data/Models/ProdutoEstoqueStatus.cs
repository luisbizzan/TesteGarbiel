using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum ProdutoEstoqueStatusEnum
    {
        Ativo = 1,
        OportunidadeNegocio = 398,
        LiquidacaoEstoque = 399,
        ForaLinha = 400
    }

    public class ProdutoEstoqueStatus 
    {
        [Key]
        [Index(IsUnique = true)]
        [Required]
        public ProdutoEstoqueStatusEnum IdProdutoEstoqueStatus { get; set; }

        [StringLength(50)]
        [Index(IsUnique = true)]
        [Required]
        public string Descricao { get; set; }
    }
}

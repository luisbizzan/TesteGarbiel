using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class Produto
    {
        [Key]
        [Required]
        public long IdProduto { get; set; }

        [Required]
        [StringLength(40)]
        public string Descricao { get; set; }

        [Required]
        [StringLength(50)]
        public string Referencia { get; set; }

        [Required]
        public long IdUnidadeMedida { get; set; }

        [Required]
        public decimal PesoBruto { get; set; }

        [Required]
        public decimal PesoLiquido { get; set; }

        [Required]
        public long CodigoIntegracao { get; set; }

        public long CodigoFabricante { get; set; }

        [StringLength(80)]
        public string NomeFabricante { get; set; }

        [StringLength(40)]
        public string DescricaoNFE { get; set; }

        [Required]
        public int CodigoProdutoNFE { get; set; }

        [Required]
        public decimal Largura { get; set; }

        [Required]
        public decimal Altura { get; set; }

        [Required]
        public decimal Comprimento { get; set; }

        public decimal MetroCubico { get; set; }

        [Required]
        [StringLength(200)]
        public string EnderecoImagem { get; set; }

        [Required]
        public decimal MultiploVenda { get; set; }

        [StringLength(30)]
        public string ReferenciaFornecedor { get; set; }

        [StringLength(50)]
        public string SKU { get; set; }

        [Required]
        public bool Ativo { get; set; }

        [ForeignKey(nameof(IdUnidadeMedida))]
        public UnidadeMedida UnidadeMedida { get; set; }
    }
}

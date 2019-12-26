using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class Produto
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]        
        public long IdProduto { get; set; }

        [Required]
        [Index]
        public long CodigoIntegracao { get; set; }

        public long? CodigoFabricante { get; set; }

        [Required]
        [StringLength(40)]
        [Index]
        public string Descricao { get; set; }

        [StringLength(50)]
        [Index]
        public string Referencia { get; set; }

        [StringLength(80)]
        public string NomeFabricante { get; set; }
     
        [StringLength(200)]
        public string EnderecoImagem { get; set; }

        [StringLength(30)]
        public string ReferenciaFornecedor { get; set; }


        [StringLength(50)]
        public string CodigoBarras { get; set; }

        [Required]
        public int CodigoProdutoNFE { get; set; }

        [Required]
        public decimal PesoBruto { get; set; }

        [Required]
        public decimal PesoLiquido { get; set; }

        public decimal? Largura { get; set; }

        public decimal? Altura { get; set; }

        public decimal? Comprimento { get; set; }

        public decimal? MetroCubico { get; set; }

        public decimal MultiploVenda { get; set; }

        [Required]
        public bool Ativo { get; set; }
    }
}

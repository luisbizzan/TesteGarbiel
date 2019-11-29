using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class Fornecedor
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdFornecedor { get; set; }

        [Required]
        [Index]
        public long CodigoIntegracao { get; set; }

        [Required]
        [Index]
        [StringLength(180)]
        public string NomeFantasia { get; set; }

        [Index]
        [StringLength(75)]
        public string RazaoSocial { get; set; }

        [Index]
        [StringLength(14)]
        public string CNPJ { get; set; }

        [Required]
        public bool Ativo { get; set; }
    }
}

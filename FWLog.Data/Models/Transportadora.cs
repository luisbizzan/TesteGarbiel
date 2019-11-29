using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class Transportadora
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdTransportadora { get; set; }

        [Required]
        [StringLength(180)]
        public string NomeFantasia { get; set; }

        [Required]
        [StringLength(75)]
        [Index]
        public string RazaoSocial { get; set; }

        [Required]
        [StringLength(14)]
        [Index]
        public string CNPJ { get; set; }

        [Required]        
        public long CodigoIntegracao { get; set; }

        [Required]
        public bool Ativo { get; set; }
    }
}
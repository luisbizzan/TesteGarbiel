using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models
{
    public class Transportadora
    {
        [Key]
        [Required]
        public long IdTransportadora { get; set; }

        [Required]
        [StringLength(180)]
        public string NomeFantasia { get; set; }

        [Required]
        [StringLength(75)]
        public string RazaoSocial { get; set; }

        [Required]
        [StringLength(14)]
        public string CNPJ { get; set; }

        [Required]
        public long CodigoIntegracao { get; set; }

        [Required]
        public bool Ativo { get; set; }
    }
}
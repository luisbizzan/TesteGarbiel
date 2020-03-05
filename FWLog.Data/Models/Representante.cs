using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class Representante
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdRepresentante { get; set; }

        [Required]
        [Index]
        public long CodigoIntegracao { get; set; }

        [Required]
        [StringLength(180)]
        public string Nome { get; set; }

        [Required]
        public bool Ativo { get; set; }
    }
}

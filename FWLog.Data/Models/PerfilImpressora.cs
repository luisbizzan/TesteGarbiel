using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class PerfilImpressora
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdPerfilImpressora { get; set; }

        [Index]
        [Required]
        public long IdEmpresa { get; set; }

        [StringLength(50)]
        [Required]
        public string Nome { get; set; }

        [Required]
        public bool Ativo { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }
    }
}

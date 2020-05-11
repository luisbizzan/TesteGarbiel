using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class CentenaVolume
    {
        [Required]
        public long IdEmpresa { get; set; }
        [Required]
        [Key]
        public int Numero { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }
    }
}

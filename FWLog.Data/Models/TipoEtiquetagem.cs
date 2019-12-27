using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum TipoEtiquetagemEnum
    {
        Individual = 1,
        Personalizada = 2,
        SAC = 3
    }

    public class TipoEtiquetagem
    {
        [Key]
        [Index(IsUnique = true)]
        [Required]
        public TipoEtiquetagemEnum IdTipoEtiquetagem { get; set; }

        [StringLength(50)]
        [Index(IsUnique = true)]
        [Required]
        public string Descricao { get; set; }
    }
}

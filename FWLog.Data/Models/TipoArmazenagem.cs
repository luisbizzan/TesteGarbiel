using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum TipoArmazenagemEnum
    {
        Desconhecido = 0,
        Volume = 1,
        Item = 2
    }

    public class TipoArmazenagem
    {
        [Key]
        [Index(IsUnique = true)]
        [Required]
        public TipoArmazenagemEnum IdTipoArmazenagem { get; set; }
        [Index(IsUnique = true)]
        [Required]
        [StringLength(50)]
        public string Descricao { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class TipoArmazenagem
    {
        [Index(IsUnique = true)]
        [Required]
        public int IdTipoArmazenagem { get; set; }
        [Index(IsUnique = true)]
        [Required]
        [StringLength(200)]
        public string Descricao { get; set; }
    }
}

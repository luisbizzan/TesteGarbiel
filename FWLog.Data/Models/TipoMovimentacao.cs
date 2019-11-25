using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class TipoMovimentacao
    {
        [Index(IsUnique = true)]
        [Required]
        public int IdTipoMovimentacao { get; set; }
        [Index(IsUnique = true)]
        [Required]
        [StringLength(200)]
        public string Descricao { get; set; }
    }
}

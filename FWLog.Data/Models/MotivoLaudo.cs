using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class MotivoLaudo
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdMotivoLaudo { get; set; }

        [Required]
        [StringLength(30)]
        [Index(IsUnique = true)]
        public string Descricao { get; set; }
        
        [Required]
        public bool Ativo { get; set; }
    }
}

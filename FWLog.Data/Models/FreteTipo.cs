using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class FreteTipo
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdFreteTipo { get; set; }

        [Required]
        [StringLength(100)]
        [Index]
        public string Descricao { get; set; }

        [Required]
        [StringLength(15)]
        [Index]
        public string Sigla { get; set; }
    }
}

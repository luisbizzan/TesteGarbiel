using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class UnidadeMedida
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdUnidadeMedida { get; set; }

        [Required]
        [StringLength(30)]
        [Index]
        public string Descricao { get; set; }

        [Required]
        [StringLength(2)]
        [Index]
        public string Sigla { get; set; }
    }
}

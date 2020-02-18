using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum GarantiaConferenciaTipoEnum
    {
        Desconhecido = 0
    }

    public class GarantiaConferenciaTipo
    {
        [Key]
        [Required]
        public GarantiaConferenciaTipoEnum IdGarantiaConferenciaTipo { get; set; }

        [Required]
        [StringLength(20)]
        [Index(IsUnique = true)]
        public string Descricao { get; set; }
    }
}

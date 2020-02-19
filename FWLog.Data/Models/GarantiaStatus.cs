using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum GarantiaStatusEnum
    {
        Desconhecido = 0
    }

    public class GarantiaStatus
    {
        [Key]
        [Required]
        public GarantiaStatusEnum IdGarantiaStatus { get; set; }

        [Required]
        [StringLength(20)]
        [Index(IsUnique=true)]
        public string Descricao { get; set; }
    }
}

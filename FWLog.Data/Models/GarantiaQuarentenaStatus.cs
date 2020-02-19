using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum GarantiaQuarentenaStatusEnum
    {
        Desconhecido = 0
    }

    public class GarantiaQuarentenaStatus
    {
        [Key]
        [Required]
        public GarantiaQuarentenaStatusEnum IdGarantiaQuarentenaStatus { get; set; }

        [Required]
        [StringLength(20)]
        [Index(IsUnique = true)]
        public string Descricao { get; set; }
    }
}

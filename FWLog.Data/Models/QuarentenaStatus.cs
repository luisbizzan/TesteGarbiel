using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models
{
    public enum QuarentenaStatusEnum
    {
        Aberto = 1,
        Retirado = 2,
        EncaminhadoAuditoria = 3,
        Finalizado = 4
    }

    public class QuarentenaStatus
    {
        [Key]
        [Required]
        public QuarentenaStatusEnum IdQuarentenaStatus { get; set; }

        [Required]
        public string Descricao { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum NotaRecebimentoStatusEnum
    {
        Registrado   = 1,
        Sincronizado = 2
    }

    public class NotaRecebimentoStatus
    {
        [Key]
        [Index(IsUnique = true)]
        [Required]
        public NotaRecebimentoStatusEnum IdNotaRecebimentoStatus { get; set; }

        [StringLength(50)]
        [Index(IsUnique = true)]
        [Required]
        public string Descricao { get; set; }
    }
}

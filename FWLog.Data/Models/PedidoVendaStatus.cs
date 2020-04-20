using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum PedidoVendaStatusEnum
    {
        ProcessandoIntegracao = 0,
        AguardandoSeparacao = 1,
    }

    public class PedidoVendaStatus
    {
        [Key]
        [Index(IsUnique = true)]
        [Required]
        public PedidoVendaStatusEnum IdPedidoVendaStatus { get; set; }

        [StringLength(50)]
        [Index(IsUnique = true)]
        [Required]
        public string Descricao { get; set; }
    }
}

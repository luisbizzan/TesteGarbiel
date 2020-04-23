using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum PedidoVendaStatusEnum
    {
        ProcessandoIntegracao = 0,
        PendenteSeparacao = 1,
        EnviadoSeparacao = 2,
        ProcessandoSeparacao = 3,
        ConcluidaComSucesso = 4,
        PendenteCancelamento = 5,
        Cancelado = 6
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

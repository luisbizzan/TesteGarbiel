using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum PedidoStatusEnum
    {
        ProcessandoIntegracao = 1,
        Integrado = 2,
        Confirmado = 3,
        Cancelado = 4,       
    }

    public class PedidoStatus
    {
        [Key]
        [Index(IsUnique = true)]
        [Required]
        public PedidoStatusEnum IdPedidoStatus { get; set; }

        [StringLength(50)]
        [Index(IsUnique = true)]
        [Required]
        public string Descricao { get; set; }
    }
}
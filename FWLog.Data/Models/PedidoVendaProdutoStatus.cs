using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum PedidoVendaProdutoStatusEnum
    {
        ProcessandoIntegracao = 0,
        AguardandoSeparacao = 1,
        EnviadoSeparacao = 2,
    }

    public class PedidoVendaProdutoStatus
    {
        [Key]
        [Index(IsUnique = true)]
        [Required]
        public PedidoVendaProdutoStatusEnum IdPedidoVendaProdutoStatus { get; set; }

        [StringLength(50)]
        [Index(IsUnique = true)]
        [Required]
        public string Descricao { get; set; }
    }
}
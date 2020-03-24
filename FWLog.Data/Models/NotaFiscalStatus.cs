using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum NotaFiscalStatusEnum
    {
        ProcessandoIntegracao = 0,
        AguardandoRecebimento = 1,
        Recebida = 2,
        Conferida = 3,
        ConferidaDivergencia = 4,
        Confirmada = 5,
        NotaDevolucaoCriada = 6,
        NotaDevolucaoAutorizada = 7,
        Cancelada = 8
    }

    public class NotaFiscalStatus
    {
        [Key]
        [Index(IsUnique = true)]
        [Required]
        public NotaFiscalStatusEnum IdNotaFiscalStatus { get; set; }

        [StringLength(50)]
        [Index(IsUnique = true)]
        [Required]
        public string Descricao { get; set; }
    }
}

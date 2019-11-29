using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum NotaFiscalStatusEnum
    {
        [Display(Name = "Processando Integracao")]
        ProcessandoIntegracao = 0,

        [Display(Name = "Aguardando recebimento")]
        AguardandoRecebimento = 1,

        [Display(Name = "Recebida")]
        Recebida = 2,

        [Display(Name = "Conferida com sucesso")]
        Conferida = 3,

        [Display(Name = "Conferida com divergência")]
        ConferidaDivergencia = 4
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

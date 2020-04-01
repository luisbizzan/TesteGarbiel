using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum GarantiaStatusEnum
    {
        Desconhecido = 0,
        AguardandoRecebimento = 1,
        Recebido = 2,
        Conferencia = 3,
        Finalizado = 4,
        FinalizadoComDivergencia = 5,
        AguardandoCriacaoNFDevolucao = 6,
        AguardandoConfirmacaoNFDevolucao = 7,
        AguardandoAutorizacaoSefaz = 8,
        AguardandoMovimentacaoEstoque = 9,
        Estorno = 10
    }

    public class GarantiaStatus
    {
        [Key]
        [Required]
        public GarantiaStatusEnum IdGarantiaStatus { get; set; }

        [Required]
        [StringLength(30)]
        [Index(IsUnique = true)]
        public string Descricao { get; set; }
    }
}

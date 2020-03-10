using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models
{
    public enum LoteStatusEnum
    {
        Desconhecido                     = 0,
        AguardandoRecebimento            = 1,
        Recebido                         = 2,
        Conferencia                      = 3,
        Finalizado                       = 4,
        ConferidoDivergencia             = 5,
        FinalizadoDivergenciaPositiva    = 6,
        FinalizadoDivergenciaNegativa    = 7,
        FinalizadoDivergenciaTodas       = 8,
        AguardandoCriacaoNFDevolucao     = 9,
        AguardandoConfirmacaoNFDevolucao = 10,
        AguardandoAutorizacaoSefaz       = 11,
        FinalizadoDevolucaoTotal         = 12
    }

    public class LoteStatus
    {
        [Key]
        [Required]
        public LoteStatusEnum IdLoteStatus { get; set; }

        [Required]
        public string Descricao { get; set; }
    }
}

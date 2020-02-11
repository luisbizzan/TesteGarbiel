using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models
{
    public enum LoteStatusEnum
    {
        [Display(Name = "Desconhecido")]
        Desconhecido = 0,
        [Display(Name = "Ag. Recebimento")]
        AguardandoRecebimento = 1,
        [Display(Name = "Recebido")]
        Recebido = 2,
        [Display(Name = "Em conferência")]
        Conferencia = 3,
        [Display(Name = "Finalizado")]
        Finalizado = 4,
        [Display(Name = "Conferido Divergência")]
        ConferidoDivergencia = 5,
        [Display(Name = "Finalizado Diverg. (A+)")]
        FinalizadoDivergenciaPositiva = 6,
        [Display(Name = "Finalizado Diverg. (A-)")]
        FinalizadoDivergenciaNegativa = 7,
        [Display(Name = "Finalizado Diverg. (A+ A-)")]
        FinalizadoDivergenciaTodas = 8,
        [Display(Name = "Ag. Criação NF Devolução")]
        AguardandoCriacaoNFDevolucao = 9,
        [Display(Name = "Ag. Confirm. NF Devolução")]
        AguardandoConfirmacaoNFDevolucao = 10,
        [Display(Name = "Ag. Autorização Sefaz")]
        AguardandoAutorizacaoSefaz = 11
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

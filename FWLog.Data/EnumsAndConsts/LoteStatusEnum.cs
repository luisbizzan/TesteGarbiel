using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.EnumsAndConsts
{
    public enum LoteStatusEnum
    {
        Desconhecido = 0,

        [Display(Name = "Aguardando recebimento")]
        AguardandoRecebimento = 1,

        [Display(Name = "Recebido")]
        Recebido = 2,

        [Display(Name = "Em conferência")]
        Conferencia = 3,

        [Display(Name = "Finalizado")]
        Finalizado = 4,

        [Display(Name = "Conferido com divergência")]
        ConferidoDivergencia = 5,

        [Display(Name = "Finalizado com divergência (A+)")]
        FinalizadoDivergenciaPositiva = 6,

        [Display(Name = "Finalizado com divergência (A-)")]
        FinalizadoDivergenciaNegativa = 7,

        [Display(Name = "Finalizado com divergência (invertido)")]
        FinalizadoDivergenciaInvertida = 8,

        [Display(Name = "Finalizado com divergência (A+, A- e invertido)")]
        FinalizadoDivergenciaTodas = 9,
    }
}

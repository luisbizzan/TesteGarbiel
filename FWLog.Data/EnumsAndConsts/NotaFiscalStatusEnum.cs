using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.EnumsAndConsts
{
    public enum NotaFiscalStatusEnum
    {
        Desconhecido = 0,

        [Display(Name = "Aguardando recebimento")]
        AguardandoRecebimento = 1,

        [Display(Name = "Recebida")]
        Recebida = 2,

        [Display(Name = "Conferida com sucesso")]
        Conferida = 3,

        [Display(Name = "Conferida com divergência")]
        ConferidaDivergencia = 4
    }
}

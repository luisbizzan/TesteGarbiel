using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models
{
    public enum LoteStatusEnum
    {
        Desconhecido = 0,
        AguardandoRecebimento = 1,
        Recebido = 2,
        Conferencia = 3,
        Finalizado = 4,
        ConferidoDivergencia = 5,
        FinalizadoDivergenciaPositiva = 6,
        FinalizadoDivergenciaNegativa = 7,
        FinalizadoDivergenciaTodas = 8,
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

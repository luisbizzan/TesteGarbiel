using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum LoteDivergenciaStatusEnum
    {
        Desconhecido = 0,
        AguardandoTratativa = 1,
        DivergenciaTratada = 2
    }

    public class LoteDivergenciaStatus
    {
        [Key]
        [Index(IsUnique = true)]
        [Required]
        public LoteDivergenciaStatusEnum IdLoteDivergenciaStatus { get; set; }
        [Index(IsUnique = true)]
        [Required]
        [StringLength(50)]
        public string Descricao { get; set; }
    }
}

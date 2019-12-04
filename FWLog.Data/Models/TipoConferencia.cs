using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum TipoConferenciaEnum
    {
        [Display(Name = "Por Quantidade")]
        PorQuantidade = 1,

        [Display(Name = "Conferência 100%")]
        ConferenciaCemPorcento = 2       
    }

    public class TipoConferencia
    {
        [Key]
        [Index(IsUnique = true)]
        [Required]
        public TipoConferenciaEnum IdTipoConferencia { get; set; }

        [StringLength(20)]
        [Index(IsUnique = true)]
        [Required]
        public string Descricao { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum TipoConferenciaEnum
    {
        PorQuantidade = 1,
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

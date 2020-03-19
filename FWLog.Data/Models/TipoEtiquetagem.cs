using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum TipoEtiquetagemEnum
    {
        Individual         = 1,
        Personalizada      = 2,
        Lote               = 3,
        Avulso             = 4,
        Devolucao          = 5,
        Recebimento        = 6,
        RecebimentoSemNota = 7
    }

    public class TipoEtiquetagem
    {
        [Key]
        [Index(IsUnique = true)]
        [Required]
        public TipoEtiquetagemEnum IdTipoEtiquetagem { get; set; }

        [StringLength(50)]
        [Index(IsUnique = true)]
        [Required]
        public string Descricao { get; set; }
    }
}

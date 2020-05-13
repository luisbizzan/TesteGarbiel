using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum DiasDaSemanaEnum
    {
        Domingo = 0,
        SegundaFeira = 1,
        TercaFeira = 2,
        QuartaFeira = 3,
        QuintaFeira = 4,
        SextaFeira = 5,
        Sabado = 6,
    }

    public class DiasDaSemana
    {
        [Key]
        [Index(IsUnique = true)]
        [Required]
        public DiasDaSemanaEnum IdDiasDaSemana { get; set; }

        [StringLength(25)]
        [Index(IsUnique = true)]
        [Required]
        public string Descricao { get; set; }
    }
}

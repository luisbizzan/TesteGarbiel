using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum DiasDaSemanaEnum
    {
        Domingo = 1,
        SegundaFeira = 2,
        TercaFeira = 3,
        QuartaFeira = 4,
        QuintaFeira = 5,
        SextaFeira = 6,
        Sabado = 7,
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

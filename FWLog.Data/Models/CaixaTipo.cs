using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum CaixaTipoEnum
    {
        Separacao = 1,
        Expedicao = 2,
        Recebimento = 3,
        Garantia = 4,
        Devolucao = 5
    }

    public class CaixaTipo
    {
        [Key]
        [Index(IsUnique = true)]
        [Required]
        public CaixaTipoEnum IdCaixaTipo { get; set; }

        [StringLength(50)]
        [Index(IsUnique = true)]
        [Required]
        public string Descricao { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models
{
    public class UnidadeMedida
    {
        [Key]
        public int IdUnidadeMedida { get; set; }
        public string Descricao { get; set; }
    }
}

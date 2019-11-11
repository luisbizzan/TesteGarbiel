using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models
{
    public class UnidadeMedida
    {
        [Key]
        public long IdUnidadeMedida { get; set; }
        public string Descricao { get; set; }
    }
}

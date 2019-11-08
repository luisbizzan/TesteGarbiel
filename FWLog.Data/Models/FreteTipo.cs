using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models
{
    public class FreteTipo
    {
        [Key]
        public int IdFreteTipo { get; set; }
        public string Descricao { get; set; }
        public string Sigla { get; set; }
    }
}

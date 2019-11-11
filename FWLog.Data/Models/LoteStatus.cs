using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models
{
    public class LoteStatus
    {
        [Key]
        public int IdLoteStatus { get; set; }
        public string Descricao { get; set; }
    }
}

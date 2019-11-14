using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models
{
    public class NotaFiscalStatus
    {
        [Key]
        public long IdNotaFiscalStatus { get; set; }
        public string Descricao { get; set; }
    }
}

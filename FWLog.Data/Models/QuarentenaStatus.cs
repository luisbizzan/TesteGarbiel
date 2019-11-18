using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models
{
    public class QuarentenaStatus
    {
        [Key]
        public long IdQuarentenaStatus { get; set; }

        public string Descricao { get; set; }
    }
}

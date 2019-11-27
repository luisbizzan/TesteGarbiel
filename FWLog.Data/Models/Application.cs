using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models
{
    public class Application
    {
        [Key]
        public int IdApplication { get; set; }
        public string Name { get; set; }
    }
}

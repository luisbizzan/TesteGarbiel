using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Data
{
    public class Application
    {
        [Key]
        public int IdApplication { get; set; }
        public string Name { get; set; }
    }
}

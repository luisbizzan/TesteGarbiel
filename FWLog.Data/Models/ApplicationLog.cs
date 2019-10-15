using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Data
{
    public partial class ApplicationLog
    {
        [Key]
        public int IdApplicationLog { get; set; }
        public System.DateTime Created { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string IdApplication { get; set; }

        public virtual Application Application { get; set; }
    }
}

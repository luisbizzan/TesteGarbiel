using System;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Data
{
    public partial class ApplicationLog
    {
        [Key]
        public int IdApplicationLog { get; set; }
        public DateTime Created { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string IdApplication { get; set; }

        public virtual Application Application { get; set; }
    }
}

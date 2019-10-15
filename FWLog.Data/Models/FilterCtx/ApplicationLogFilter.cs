using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Data.Models.FilterCtx
{
    public class ApplicationLogFilter
    {
        public string Message { get; set; }
        public string Level { get; set; }
        public System.DateTime? CreatedStart { get; set; }
        public System.DateTime? CreatedEnd { get; set; }
    }
}

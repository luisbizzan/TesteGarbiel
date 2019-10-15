using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Data.Models.FilterCtx
{
    public class BOLogSystemFilter
    {
        public string UserName { get; set; }
        public string ActionType { get; set; }
        public string Entity { get; set; }
        public string IP { get; set; }
        public DateTime? ExecutionDateStart { get; set; }
        public DateTime? ExecutionDateEnd { get; set; }
    }
}

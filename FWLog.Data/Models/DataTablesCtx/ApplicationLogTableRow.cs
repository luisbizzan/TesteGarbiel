using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class ApplicationLogTableRow
    {
        public int IdApplicationLog { get; set; }

        public System.DateTime Created { get; set; }

        public string Level { get; set; }

        public string Message { get; set; }

        public string ApplicationName { get; set; }
    }

}

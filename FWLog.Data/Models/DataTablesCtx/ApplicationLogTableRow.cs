using System;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class ApplicationLogTableRow
    {
        public long IdApplicationLog { get; set; }

        public DateTime Created { get; set; }

        public string Level { get; set; }

        public string Message { get; set; }

        public string ApplicationName { get; set; }
    }

}

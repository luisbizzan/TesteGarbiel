using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class BOLogSystemTableRow
    {
        public long IdBOLogSystem { get; set; }
        public string UserName { get; set; }
        public string ActionType { get; set; }
        public string Description { get; set; }
        public string IP { get; set; }
        public System.DateTime ExecutionDate { get; set; }
        public string Entity { get; set; }
        public string TranslatedEntity { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FWLog.Data.Repository.BackofficeCtx.BOLogSystemRepository;

namespace FWLog.Data.Models.GeneralCtx
{
    public class BOLogSystemDetails
    {
        public long IdBOLogSystem { get; set; }
        public string UserName { get; set; }
        public string ActionType { get; set; }
        public string TranslatedActionType { get; set; }
        public string IP { get; set; }
        public DateTime ExecutionDate { get; set; }
        public string Entity { get; set; }
        public string TranslatedEntity { get; set; }

        public IEnumerable<BOLogSystemRelated> RelatedLogs { get; set; }
        public IEnumerable<BOLogSystemColumnChanges> ColumnChanges { get; set; }
    }

    public class BOLogSystemRelated
    {
        public long IdBOLogSystem { get; set; }
        public string Entity { get; set; }
        public string TranslatedEntity { get; set; }
        public string ActionType { get; set; }
    }
}

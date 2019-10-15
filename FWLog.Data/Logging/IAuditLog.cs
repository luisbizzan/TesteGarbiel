using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Data.Logging
{
    public interface IAuditLog
    {
        void AddLogsToContextAndSaveChanges(Entities entities, out int nonLogChanges);
    }
}

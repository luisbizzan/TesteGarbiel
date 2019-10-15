using FWLog.Data.EnumsAndConsts;
using DartDigital.Library.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Data.Logging
{
    public class WebApiAuditLog : IAuditLog
    {
        public void AddLogsToContextAndSaveChanges(Entities entities, out int nonLogChanges)
        {
            nonLogChanges = entities.SaveChangesWithoutLog();

            // Implementar lógica de log se necessário.
        }
    }
}

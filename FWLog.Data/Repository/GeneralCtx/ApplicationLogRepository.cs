using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class ApplicationLogRepository : GenericRepository<ApplicationLog>
    {
        public ApplicationLogRepository(Entities entities) : base(entities)
        {

        }

        public override ApplicationLog GetById(long id)
        {
            return Entities.ApplicationLog.FirstOrDefault(x => x.IdApplicationLog == id);
        }

        public IList<ApplicationLog> SearchByMessage(string message, int takeCount)
        {
            return Entities.ApplicationLog.Where(x => x.Message.Contains(message)).Take(takeCount).ToList();
        }

        public IList<ApplicationLogTableRow> SearchForDataTable(DataTableFilter<ApplicationLogFilter> model, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.ApplicationLog.Count();


            IQueryable<ApplicationLogTableRow> query = (from appLog in Entities.ApplicationLog.AsNoTracking()
                                                        join app in Entities.Application on appLog.IdApplication equals app.IdApplication
                                                        select new ApplicationLogTableRow
                                                        {
                                                            IdApplicationLog = appLog.IdApplicationLog,
                                                            Created = appLog.Created,
                                                            Level = appLog.Level,
                                                            Message = appLog.Message,
                                                            ApplicationName = app.Name
                                                        });
                   
            if (!String.IsNullOrEmpty(model.CustomFilter.Message))
            {
                query = query.Where(x => x.Message.ToLower().Contains(model.CustomFilter.Message.ToLower()));
            }

            if (!String.IsNullOrEmpty(model.CustomFilter.Level))
            {
                query = query.Where(x => x.Level.Contains(model.CustomFilter.Level));
            }

            if (model.CustomFilter.CreatedStart.HasValue)
            {
                DateTime date = model.CustomFilter.CreatedStart.Value.Date;
                query = query.Where(x => DbFunctions.TruncateTime(x.Created) >= date);
            }

            if (model.CustomFilter.CreatedEnd.HasValue)
            {
                DateTime date = model.CustomFilter.CreatedEnd.Value.Date;
                query = query.Where(x => DbFunctions.TruncateTime(x.Created) <= date);
            }

            // Quantidade total de registros com filtros aplicados, sem Skip() e Take().
            totalRecordsFiltered = query.Count();

            query = query
                .OrderBy(model.OrderByColumn, model.OrderByDirection)
                .Skip(model.Start)
                .Take(model.Length);

            return query.ToList();
        }
    }
}

using FWLog.Data.EnumsAndConsts;
using FWLog.Data.GlobalResources.Entity;
using FWLog.Data.Logging;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Models.GeneralCtx;
using FWLog.Data.Repository.CommonCtx;
using DartDigital.Library.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Res = FWLog.Data.GlobalResources.General.GeneralStrings;
using FWLog.Data.Models;

namespace FWLog.Data.Repository.BackofficeCtx
{
    public class BOLogSystemRepository : BaseRepository
    {
        public BOLogSystemRepository(Entities entities) : base(entities)
        {

        }

        public void Add(BOLogSystem entity)
        {
            // Necessário preencher o escopo para identificar depois quais operações ocorreram junto.
            if (String.IsNullOrEmpty(entity.ScopeIdentifier) && Entities.AuditLog is BackOfficeAuditLog)
            {
                entity.ScopeIdentifier = (Entities.AuditLog as BackOfficeAuditLog).ScopeIdentifier.ToString();
            }

            Entities.BOLogSystem.Add(entity);
        }

        public BOLogSystem GetById(long id)
        {
            return Entities.BOLogSystem.Include(x => x.AspNetUsers).FirstOrDefault(x => x.IdBOLogSystem == id);
        }

        /// <summary>
        /// Retorna todas entidades que estão sendo logadas.
        /// </summary>
        /// <returns></returns>
        public IList<LogEntity> GetLogEntities()
        {
            var objectContext = ((IObjectContextAdapter)Entities).ObjectContext;

            var mdw = objectContext.MetadataWorkspace;
            IEnumerable<string> entityNames = objectContext.MetadataWorkspace.GetItems<EntityType>(DataSpace.CSpace).Select(x => x.Name);

            var logEntities = new List<LogEntity>();

            foreach (string name in entityNames)
            {
                MetadataTypeAttribute metadataTypeAttr = GetMetadataTypeForEntity(name);

                if (metadataTypeAttr == null)
                {
                    continue;
                }

                LogAttribute logAttr = metadataTypeAttr.MetadataClassType.GetCustomAttributes(typeof(LogAttribute), false).FirstOrDefault() as LogAttribute;

                if (logAttr == null)
                {
                    continue;
                }

                logEntities.Add(new LogEntity { OriginalName = name, TranslatedName = GetTranslationForEntity(metadataTypeAttr, name) });
            }

            return logEntities;
        }

        public BOLogSystemDetails GetDetailsById(long id)
        {
            BOLogSystem log = Entities.BOLogSystem.Include(x => x.AspNetUsers).FirstOrDefault(x => x.IdBOLogSystem == id);

            if (log == null)
            {
                return null;
            }

            MetadataTypeAttribute entityMetadataType = GetMetadataTypeForEntity(log.Entity);

            var details = new BOLogSystemDetails
            {
                IdBOLogSystem = log.IdBOLogSystem,
                UserName = log.AspNetUsers.UserName,
                ActionType = log.ActionType,
                TranslatedActionType = ActionTypeNames.GetAll().First(x => x.Value == log.ActionType).DisplayName,
                IP = log.IP,
                ExecutionDate = log.ExecutionDate,
                Entity = log.Entity,
                TranslatedEntity = GetTranslationForEntity(entityMetadataType, log.Entity),
                ColumnChanges = GetColumnChanges(log)
            };

            IEnumerable<BOLogSystemRelated> scopeEntities = Entities.BOLogSystem
                .Where(x => x.ScopeIdentifier == log.ScopeIdentifier && x.IdBOLogSystem != log.IdBOLogSystem)
                .Select(x => new BOLogSystemRelated
                {
                    IdBOLogSystem = x.IdBOLogSystem,
                    ActionType = x.ActionType,
                    Entity = x.Entity,
                    TranslatedEntity = null
                })
                .ToList();

            foreach (BOLogSystemRelated item in scopeEntities)
            {
                MetadataTypeAttribute itemEntityMetadataType = GetMetadataTypeForEntity(item.Entity);
                item.TranslatedEntity = GetTranslationForEntity(itemEntityMetadataType, item.Entity);
            }

            details.RelatedLogs = scopeEntities;

            return details;
        }

        public IList<BOLogSystemTableRow> SearchForDataTable(DataTableFilter<BOLogSystemFilter> model, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.ApplicationLog.Count();

            IQueryable<BOLogSystemTableRow> query = Entities.BOLogSystem.AsNoTracking()
                .Select(x => new BOLogSystemTableRow
                {
                    IdBOLogSystem = x.IdBOLogSystem,
                    UserName = x.AspNetUsers.UserName,
                    ActionType = x.ActionType,
                    IP = x.IP,
                    ExecutionDate = x.ExecutionDate,
                    Entity = x.Entity
                });

            if (!String.IsNullOrEmpty(model.CustomFilter.UserName))
            {
                query = query.Where(x => x.UserName.Contains(model.CustomFilter.UserName));
            }

            if (!String.IsNullOrEmpty(model.CustomFilter.ActionType))
            {
                query = query.Where(x => x.ActionType.Contains(model.CustomFilter.ActionType));
            }

            if (!String.IsNullOrEmpty(model.CustomFilter.Entity))
            {
                query = query.Where(x => x.Entity == model.CustomFilter.Entity);
            }

            if (!String.IsNullOrEmpty(model.CustomFilter.IP))
            {
                query = query.Where(x => x.IP.Contains(model.CustomFilter.IP));
            }

            if (model.CustomFilter.ExecutionDateStart.HasValue)
            {
                DateTime date = model.CustomFilter.ExecutionDateStart.Value.Date;
                query = query.Where(x => DbFunctions.TruncateTime(x.ExecutionDate) >= date);
            }

            if (model.CustomFilter.ExecutionDateEnd.HasValue)
            {
                DateTime date = model.CustomFilter.ExecutionDateEnd.Value.Date;
                query = query.Where(x => DbFunctions.TruncateTime(x.ExecutionDate) <= date);
            }

            totalRecordsFiltered = query.Count();

            query = query
                .OrderBy(model.OrderByColumn, model.OrderByDirection)
                .Skip(model.Start)
                .Take(model.Length);

            List<BOLogSystemTableRow> result = query.ToList();

            foreach (BOLogSystemTableRow item in result)
            {
                MetadataTypeAttribute entityMetadataType = GetMetadataTypeForEntity(item.Entity);
                item.ActionType = ActionTypeNames.GetAll().First(x => x.Value == item.ActionType).DisplayName;
                item.Entity = GetTranslationForEntity(entityMetadataType, item.Entity);
            }

            return result;
        }

        private IEnumerable<BOLogSystemColumnChanges> GetColumnChanges(BOLogSystem log)
        {
            Dictionary<string, string> oldEntityDictionary = DeserializeEntity(log.OldEntity, log);
            Dictionary<string, string> newEntityDictionary = DeserializeEntity(log.NewEntity, log);
            var columnChanges = new List<BOLogSystemColumnChanges>();

            MetadataTypeAttribute entityMetadataType = GetMetadataTypeForEntity(log.Entity);

            // New Entity
            foreach (var item in newEntityDictionary)
            {
                columnChanges.Add(new BOLogSystemColumnChanges
                {
                    OriginalName = item.Key,
                    TranslatedName = GetTranslationForProperty(entityMetadataType, item.Key),
                    NewValue = item.Value
                });
            }

            // Old Entity
            foreach (var item in oldEntityDictionary)
            {
                BOLogSystemColumnChanges currentChanges = columnChanges.FirstOrDefault(x => x.OriginalName == item.Key);

                if (currentChanges != null)
                {
                    currentChanges.OldValue = item.Value;
                }
                else
                {
                    columnChanges.Add(new BOLogSystemColumnChanges
                    {
                        OriginalName = item.Key,
                        TranslatedName = GetTranslationForProperty(entityMetadataType, item.Key),
                        OldValue = item.Value
                    });
                }
            }

            return columnChanges;
        }

        private Dictionary<string, string> DeserializeEntity(string entityJson, BOLogSystem log)
        {
            string dataNamespace = Entities.GetType().Namespace;
            string entityName = log.Entity;

            if (entityJson.NullOrEmpty())
            {
                return new Dictionary<string, string>();
            }

            Dictionary<string, string> entityDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(entityJson);

            if (entityDic.NullOrEmpty() || !entityDic.Any())
            {
                return new Dictionary<string, string>();
            }

            var entityType = Type.GetType(dataNamespace + "." + entityName + "," + dataNamespace);
            var entityTypeMetadata = Type.GetType(dataNamespace + "." + entityName + "Metadata," + dataNamespace);

            var entity = JsonConvert.DeserializeObject(entityJson, entityType);

            var tempDic = entityDic.ToDictionary(entry => entry.Key, entry => entry.Value);

            foreach (var property in tempDic.Keys)
            {
                PropertyInfo propertyInfo = entityType != null ? entityType.GetProperty(property) : null;
                PropertyInfo propertyInfoMetadata = entityTypeMetadata != null ? entityTypeMetadata.GetProperty(property) : null;

                if (!entityDic[property].NullOrEmpty())
                {
                    if (propertyInfo.PropertyType == typeof(Boolean) || propertyInfo.PropertyType == typeof(Boolean?) ||
                        propertyInfo.PropertyType == typeof(bool) || propertyInfo.PropertyType == typeof(bool?))
                    {
                        if (entityDic[property].Equals("0"))
                            entityDic[property] = Res.No;
                        else
                            entityDic[property] = Res.Yes;
                    }
                    else if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
                    {
                        DateTime myDate = DateTime.ParseExact(entityDic[property], "yyyy-MM-ddTHH:mm:ss.FFF", System.Globalization.CultureInfo.InvariantCulture);
                        entityDic[property] = myDate.ToString();
                    }
                    else if (propertyInfo.PropertyType == typeof(TimeSpan) || propertyInfo.PropertyType == typeof(TimeSpan?))
                    {
                        TimeSpan timespan = TimeSpan.Parse(entityDic[property]);
                        entityDic[property] = new DateTime(timespan.Ticks).ToString("HH:mm");
                    }
                    else if (propertyInfo.PropertyType == typeof(Decimal) || propertyInfo.PropertyType == typeof(Decimal?))
                    {
                        //Decimal value = Decimal.Parse(entityDic[property]);
                        Decimal value = Decimal.Parse(entityDic[property], System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign, new CultureInfo("en-US"));
                        entityDic[property] = value.ToString("N2");
                    }
                    else if (propertyInfo.PropertyType == typeof(Double) || propertyInfo.PropertyType == typeof(Double?))
                    {
                        Double value = Double.Parse(entityDic[property], System.Globalization.NumberStyles.AllowDecimalPoint, new CultureInfo("en-US"));
                        entityDic[property] = value.ToString("N2");
                    }
                }

                if (propertyInfoMetadata != null)
                {
                    var removeEntityFromLog = entityTypeMetadata.GetCustomAttributes(typeof(RemoveFromLogAttribute), false).FirstOrDefault() as RemoveFromLogAttribute;

                    if (removeEntityFromLog != null)
                        continue;

                    var displayNameAttributeMetadata = propertyInfoMetadata.GetCustomAttributes(typeof(DisplayNameAttribute), false).FirstOrDefault() as DisplayNameAttribute;

                    if (displayNameAttributeMetadata != null) //Update key with the metadata value
                    {
                        string value = entityDic[property];
                        entityDic.Remove(property);
                        entityDic[displayNameAttributeMetadata.DisplayName] = value;
                    }
                }

            }

            return entityDic;
        }

        private string GetTranslationForProperty(MetadataTypeAttribute entityMetadataType, string propName)
        {
            if (entityMetadataType == null)
            {
                return propName;
            }

            PropertyInfo info = entityMetadataType.MetadataClassType.GetProperty(propName);

            if (info == null)
            {
                return propName;
            }

            var logAttr = (LogAttribute)info.GetCustomAttributes(typeof(LogAttribute)).FirstOrDefault();

            if (logAttr == null)
            {
                return propName;
            }

            string displayName = logAttr.GetDisplayName();
            return String.IsNullOrEmpty(displayName) ? propName : displayName;
        }

        private string GetTranslationForEntity(MetadataTypeAttribute entityMetadataType, string entityName)
        {
            if (entityMetadataType == null)
            {
                return entityName;
            }

            var logAttr = (LogAttribute)entityMetadataType.MetadataClassType.GetCustomAttributes(typeof(LogAttribute), false).FirstOrDefault();

            if (logAttr == null)
            {
                return entityName;
            }

            string displayName = logAttr.GetDisplayName();
            return String.IsNullOrEmpty(displayName) ? entityName : displayName;
        }

        private MetadataTypeAttribute GetMetadataTypeForEntity(string entityName)
        {
            string entitiesNamespace = Entities.GetType().Namespace;
            Type entityType = Type.GetType(String.Concat(entitiesNamespace, ".", entityName, ",", entitiesNamespace));

            if (entityType == null)
            {
                return null;
            }

            return entityType.GetCustomAttributes(typeof(MetadataTypeAttribute), false).FirstOrDefault() as MetadataTypeAttribute;
        }
    }
}

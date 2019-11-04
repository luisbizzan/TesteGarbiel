using FWLog.Data.EnumsAndConsts;
using DartDigital.Library.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FWLog.Data.Models;

namespace FWLog.Data.Logging
{
    public class BackOfficeAuditLog : IAuditLog
    {
        public Guid ScopeIdentifier { get; private set; }
        IBackOfficeUserInfo _userInfo;

        public BackOfficeAuditLog(IBackOfficeUserInfo userInfo)
        {
            ScopeIdentifier = Guid.NewGuid();
            _userInfo = userInfo;
        }

        public void AddLogsToContextAndSaveChanges(Entities entities, out int nonLogChanges)
        {
            // Se o usuário não estiver autenticado, não há como identificar quem está fazendo alteração.
            // Portanto não é salvo o log.
            if (!_userInfo.IsAuthenticated)
            {
                nonLogChanges = entities.SaveChangesWithoutLog();
                return;
            }

            List<BOLogSystem> boLogSystemList = new List<BOLogSystem>();
            List<ObjectStateEntry> objectStateEntryList = new List<ObjectStateEntry>();

            try
            {
                entities.ChangeTracker.DetectChanges();

                var entries = entities.ChangeTracker.Entries().Where(c => (c.State == EntityState.Added)
                    || (c.State == EntityState.Deleted)
                    || (c.State == EntityState.Modified));

                var context = ((IObjectContextAdapter)entities).ObjectContext;
                objectStateEntryList = context.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified | EntityState.Deleted).ToList();

                foreach (var entry in entries)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            boLogSystemList.Add(CreateLogForEntry(entry, ActionTypeNames.Add.Value));
                            break;
                        case EntityState.Deleted:
                            boLogSystemList.Add(CreateLogForEntry(entry, ActionTypeNames.Delete.Value));
                            break;
                        case EntityState.Modified:
                            boLogSystemList.Add(CreateLogForEntry(entry, ActionTypeNames.Edit.Value));
                            break;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                nonLogChanges = entities.SaveChangesWithoutLog();

                if (boLogSystemList != null)
                {
                    for (int i = 0; i < objectStateEntryList.Count; i++)
                    {
                        if (boLogSystemList[i] != null && boLogSystemList[i].ActionType == ActionTypeNames.Add.Value)
                        {
                            var entityKey = objectStateEntryList[i].EntityKey.EntityKeyValues[0];
                            var bogLogSystem = boLogSystemList[i];
                            bogLogSystem.IdBOLogSystem = int.Parse(entityKey.Value.ToString());
                            bogLogSystem.NewEntity = bogLogSystem.NewEntity.Replace("#Id#", entityKey.Value.ToString());
                        }
                    }

                    foreach (var boLogSystem in boLogSystemList)
                    {
                        if (boLogSystem != null)
                        {
                            entities.BOLogSystem.Add(boLogSystem);
                        }
                    }

                    entities.SaveChangesWithoutLog();
                }
            }
        }


        private BOLogSystem CreateLogForEntry(DbEntityEntry entry, string actionType)
        {
            var description = new StringBuilder();
            var entity = entry.Entity;
            Type entityType = entity.GetType();
            Type entityTrueType = entityType.BaseType.Name != "Object" ? entityType.BaseType : entityType;

            var oldEntity = Activator.CreateInstance(entityTrueType);
            var entityName = entityTrueType.Name;
           
            var logAttr = entityTrueType.GetCustomAttributes(typeof(LogAttribute), false).FirstOrDefault();

            if (logAttr == null)
            {
                return null;
            }

            var oldEntityJson = new StringBuilder();
            var newEntityJson = new StringBuilder("{");
            long idEntity = 0;

            if (actionType == ActionTypeNames.Edit.Value)
            {
                foreach (var databaseColumn in DbHelper.GetDatabaseColumnsNames(entityTrueType))
                {
                    var propertyInfo = entityTrueType.GetProperty(databaseColumn);
                    propertyInfo.SetValue(oldEntity, (entry.OriginalValues[databaseColumn] == DBNull.Value ? null : entry.OriginalValues[databaseColumn]), null);
                }

                oldEntityJson.Append("{");
                int propertyCount = 0;

                foreach (var property in entry.CurrentValues.PropertyNames)
                {
                    var propertyInfo = entityTrueType.GetProperty(property);
                    
                    LogAttribute propLogAttr = (LogAttribute)propertyInfo.GetCustomAttributes(typeof(LogAttribute), false).FirstOrDefault();

                    if (propLogAttr == null)
                    {
                        continue;
                    }

                    var originalValueProperty = entry.OriginalValues[property];
                    var currentValueProperty = entry.CurrentValues[property];
                    string originalValue = PreparePropertyValue(originalValueProperty);

                    if (property.Equals(string.Concat("Id", entityName)) || (propertyCount == 0 && property.Contains("Id")))
                    {
                        idEntity = long.Parse(originalValue);
                    }

                    var originalFormatedValue = FormatValueForJson(propertyInfo, originalValueProperty);
                    oldEntityJson.Append(string.Format("\"{0}\":{1},", propertyInfo.Name, originalFormatedValue));

                    var currentFormatedValue = FormatValueForJson(propertyInfo, currentValueProperty);
                    newEntityJson.Append(string.Format("\"{0}\":{1},", propertyInfo.Name, currentFormatedValue));

                    propertyCount++;
                }

                oldEntityJson = oldEntityJson.Remove(oldEntityJson.Length - 1, 1);
                oldEntityJson.Append("}");
            }
            else
            {
                int propertyCount = 0;
                foreach (var property in DbHelper.GetDatabaseColumnsNames(entityTrueType))
                {
                    var propertyInfo = entityTrueType.GetProperty(property);
                    LogAttribute propLogAttr = (LogAttribute)propertyInfo.GetCustomAttributes(typeof(LogAttribute), false).FirstOrDefault();

                    if (propLogAttr == null)
                    {
                        continue;
                    }

                    Object valueProperty = null;

                    if (actionType == ActionTypeNames.Delete.Value)
                    {
                        valueProperty = entry.OriginalValues[property];
                    }
                    else
                    {
                        valueProperty = entry.CurrentValues[property];
                    }

                    var originalValue = PreparePropertyValue(valueProperty);

                    if (property.Equals(string.Concat("Id", entityName)) || (propertyCount == 0 && property.Contains("Id")))
                    {
                        idEntity = long.Parse(originalValue);

                        if (actionType == ActionTypeNames.Add.Value)
                        {
                            valueProperty = "#Id#";
                        }
                    }

                    var currentFormatedValue = FormatValueForJson(propertyInfo, valueProperty);
                    newEntityJson.Append(string.Format("\"{0}\":{1},", propertyInfo.Name, currentFormatedValue));

                    propertyCount++;
                }
            }

            newEntityJson = newEntityJson.Remove(newEntityJson.Length - 1, 1);
            newEntityJson.Append("}");

            var boLogSystem = new BOLogSystem
            {
                ActionType = actionType,
                Entity = entityName,
                ExecutionDate = DateTime.UtcNow,
                IP = _userInfo.IP,
                OldEntity = oldEntityJson.ToString().Equals("") ? null : oldEntityJson.ToString(),
                NewEntity = newEntityJson.ToString(),
                ScopeIdentifier = ScopeIdentifier.ToString()
            };

            boLogSystem.SetUserId(_userInfo.UserId);

            return boLogSystem;
        }

        public Object FormatValueForJson(PropertyInfo propertyInfo, Object currentValueProperty)
        {
            Object value = currentValueProperty;

            if (propertyInfo.PropertyType == typeof(string) || propertyInfo.PropertyType == typeof(String) || propertyInfo.PropertyType == typeof(TimeSpan)
                || propertyInfo.PropertyType == typeof(TimeSpan?) || propertyInfo.PropertyType == typeof(Guid) || propertyInfo.PropertyType == typeof(Guid?))
            {
                value = currentValueProperty == null ? "\"\"" : "\"" + currentValueProperty + "\"";
            }
            else if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
            {
                if (currentValueProperty != null)
                {
                    DateTime dt = (DateTime)currentValueProperty;
                    value = "\"" + dt.ToString("yyyy-MM-ddTHH:mm:ss.FFF") + "\"";
                }
                else
                {
                    value = currentValueProperty == null ? "\"\"" : "\"" + currentValueProperty + "\"";
                }
            }
            else if (propertyInfo.PropertyType == typeof(Boolean) || propertyInfo.PropertyType == typeof(Boolean?) ||
                propertyInfo.PropertyType == typeof(bool) || propertyInfo.PropertyType == typeof(bool?))
            {
                if (currentValueProperty != null)
                {
                    value = (bool)currentValueProperty ? 1 : 0;
                }
            }
            else
            {
                if (value == null)
                {
                    value = "\"\"";
                }
                else if (value.ToString().Contains(","))
                {
                    value = value.ToString().Replace(',', '.');
                }
            }

            return value;
        }

        public string PreparePropertyValue(Object propertyValue)
        {
            string preparedValue = "(Vazio)";

            if (propertyValue != null && propertyValue != DBNull.Value)
            {
                if (propertyValue.ToString() == "True")
                {
                    preparedValue = "Sim";
                }
                else if (propertyValue.ToString() == "False")
                {
                    preparedValue = "Não";
                }
                else
                {
                    preparedValue = propertyValue.ToString();
                }
            }

            return preparedValue;
        }
    }
}

using System;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data
{
    public class BOLogSystem
    {
        [Key]
        public long IdBOLogSystem { get; set; }        
        public string AspNetUsersId { get; set; }
        public string ActionType { get; set; }
        public string IP { get; set; }
        public DateTime ExecutionDate { get; set; }
        public string Entity { get; set; }
        public string OldEntity { get; set; }
        public string NewEntity { get; set; }
        public string ScopeIdentifier { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        public void SetUserId(object value)
        {
            PropertyInfo userIdProp = typeof(BOLogSystem).GetProperty(nameof(AspNetUsersId));
            Type userIdType = userIdProp.PropertyType;

            userIdProp.SetValue(this, value);
        }
    }
}

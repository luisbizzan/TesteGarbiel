using System;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Data
{
    public partial class BOLogSystem
    {
        [Key]
        public long IdBOLogSystem { get; set; }
        public string UserId { get; set; }
        public string ActionType { get; set; }
        public string IP { get; set; }
        public System.DateTime ExecutionDate { get; set; }
        public string Entity { get; set; }
        public string OldEntity { get; set; }
        public string NewEntity { get; set; }
        public string ScopeIdentifier { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        public void SetUserId(object value)
        {
            PropertyInfo userIdProp = typeof(BOLogSystem).GetProperty(nameof(UserId));
            Type userIdType = userIdProp.PropertyType;

            userIdProp.SetValue(this, value);
        }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Data
{
    public class Application
    {
        public Application()
        {
            AspNetRoles = new HashSet<AspNetRoles>();
            AspNetUsers = new HashSet<AspNetUsers>();
            ApplicationSession = new HashSet<ApplicationSession>();
            AspNetPermissions = new HashSet<AspNetPermissions>();
        }

        [Key]
        public int IdApplication { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AspNetRoles> AspNetRoles { get; set; }
        public virtual ICollection<AspNetUsers> AspNetUsers { get; set; }
        public virtual ICollection<ApplicationSession> ApplicationSession { get; set; }
        public virtual ICollection<AspNetPermissions> AspNetPermissions { get; set; }
    }
}

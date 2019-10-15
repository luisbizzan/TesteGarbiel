using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Data
{
    public partial class Application
    {
        public Application()
        {
            this.AspNetRoles = new HashSet<AspNetRoles>();
            this.AspNetUsers = new HashSet<AspNetUsers>();
            this.ApplicationSession = new HashSet<ApplicationSession>();
            this.AspNetPermissions = new HashSet<AspNetPermissions>();
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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Data
{
    public partial class ApplicationSession
    {
        public ApplicationSession()
        {
            this.AspNetUsers = new HashSet<AspNetUsers>();
        }

        [Key]
        public int IdApplicationSession { get; set; }
        public string IdAspNetUsers { get; set; }
        public int IdApplication { get; set; }
        public System.DateTime DataLogin { get; set; }
        public System.DateTime DataUltimaAcao { get; set; }
        public Nullable<System.DateTime> DataLogout { get; set; }

        public virtual Application Application { get; set; }
        public virtual ICollection<AspNetUsers> AspNetUsers { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data
{
    public class ApplicationSession
    {
        public ApplicationSession()
        {
            AspNetUsers = new HashSet<AspNetUsers>();
        }

        [Key]
        public int IdApplicationSession { get; set; }
        public string IdAspNetUsers { get; set; }
        public int IdApplication { get; set; }
        public DateTime DataLogin { get; set; }
        public DateTime DataUltimaAcao { get; set; }
        public DateTime? DataLogout { get; set; }                
        public int CompanyId { get; set; }

        public virtual Application Application { get; set; }
        public virtual ICollection<AspNetUsers> AspNetUsers { get; set; }
    }
}

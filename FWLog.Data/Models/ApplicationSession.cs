using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data
{
    public class ApplicationSession
    {
        [Key]
        public long IdApplicationSession { get; set; }
        public string IdAspNetUsers { get; set; }
        public int IdApplication { get; set; }
        public DateTime DataLogin { get; set; }
        public DateTime DataUltimaAcao { get; set; }
        public DateTime? DataLogout { get; set; }                
        public long? CompanyId { get; set; }
    }
}

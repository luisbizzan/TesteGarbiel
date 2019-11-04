using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data
{
    [Table("APPLICATIONLOG")]
    public class ApplicationLog
    {
        [Key]
        [Column("IDAPPLICATIONLOG")]
        public long IdApplicationLog { get; set; }
        [Column("CREATED")]
        public DateTime Created { get; set; }
        [Column("LOGLEVEL")]
        public string Level { get; set; }
        [Column("MESSAGE")]
        public string Message { get; set; }
        [Column("LOGEXCEPTION")]
        public string Exception { get; set; }
        [Column("IDAPPLICATION")]
        public int IdApplication { get; set; }
    }
}

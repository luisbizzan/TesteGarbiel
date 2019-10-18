using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Data
{
    public class AspNetRoles
    {
        public AspNetRoles()
        {
            AspNetUsers = new HashSet<AspNetUsers>();
        }

        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public int ApplicationId { get; set; }

        public virtual Application Application { get; set; }
        public virtual ICollection<AspNetUsers> AspNetUsers { get; set; }
    }
}

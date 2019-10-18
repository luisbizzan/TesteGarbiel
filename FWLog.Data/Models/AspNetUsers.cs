using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Data
{
    public class AspNetUsers
    {
        public AspNetUsers()
        {
            BOLogSystem = new HashSet<BOLogSystem>();
            AspNetRoles = new HashSet<AspNetRoles>();
        }

        [Key]
        public string Id { get; set; }
        public int ApplicationId { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string UserName { get; set; }
        public int? ApplicationSessionId { get; set; }

        public virtual Application Application { get; set; }
        public virtual ICollection<BOLogSystem> BOLogSystem { get; set; }
        public virtual ApplicationSession ApplicationSession { get; set; }
        public virtual ICollection<AspNetRoles> AspNetRoles { get; set; }
    }
}

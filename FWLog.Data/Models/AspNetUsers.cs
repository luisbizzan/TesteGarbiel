using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ResEnt = Resources.EntityStrings;

namespace FWLog.Data.Models
{
    public class AspNetUsers
    {
        public AspNetUsers()
        {
            AspNetRoles = new HashSet<AspNetRoles>();
        }

        [Key]
        public string Id { get; set; }

        public string UserName { get; set; }

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
        public int ApplicationId { get; set; }
        public int? IdApplicationSession { get; set; }

        //public virtual PerfilUsuario PerfilUsuario { get; set; }

        public ICollection<AspNetRoles> AspNetRoles { get; set; }
    }
}

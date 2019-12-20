using FWLog.Data.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ResEnt = Resources.EntityStrings;

namespace FWLog.Data.Models
{
    [Log(DisplayName = nameof(ResEnt.AspNetUsers), ResourceType = typeof(ResEnt))]
    public class AspNetUsers
    {
        public AspNetUsers()
        {
            AspNetRoles = new HashSet<AspNetRoles>();
        }

        [Key]
        [Log(DisplayName = nameof(ResEnt.UserId), ResourceType = typeof(ResEnt))]
        public string Id { get; set; }

        [Log(DisplayName = nameof(ResEnt.UserName), ResourceType = typeof(ResEnt))]
        public string UserName { get; set; }

        [Log(DisplayName = nameof(ResEnt.Email), ResourceType = typeof(ResEnt))]
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

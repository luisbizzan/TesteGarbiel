﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ResEnt = Resources.EntityStrings;

namespace FWLog.Data.Models
{
    public class AspNetRoles
    {
        public AspNetRoles()
        {
            AspNetUsers = new HashSet<AspNetUsers>();
        }

        [Key]
        [Display(Name = nameof(ResEnt.RoleId), ResourceType = typeof(ResEnt))]
        public string Id { get; set; }

        [Display(Name = nameof(ResEnt.RoleName), ResourceType = typeof(ResEnt))]
        public string Name { get; set; }

        [Display(Name = nameof(ResEnt.ApplicationId), ResourceType = typeof(ResEnt))]
        public int ApplicationId { get; set; }

        public virtual Application Application { get; set; }
        public virtual ICollection<AspNetUsers> AspNetUsers { get; set; }
    }
}

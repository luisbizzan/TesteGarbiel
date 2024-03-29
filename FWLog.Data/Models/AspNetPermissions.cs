﻿using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models
{
    public class AspNetPermissions
    {
        [Key]
        public string Id { get; set; }
        public int ApplicationId { get; set; }
        public string Name { get; set; }

        public Application Application { get; set; }
    }
}

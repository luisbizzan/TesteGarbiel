using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Res = Resources.ApplicationLogStrings;

namespace FWLog.Web.Backoffice.Models.ApplicationLogCtx
{
    public class ApplicationLogDetailsViewModel
    {
        [Display(Name = nameof(Res.IdApplicationLogLabel), ResourceType = typeof(Res))]
        public int IdApplicationLog { get; set; }

        [Display(Name = nameof(Res.CreatedLabel), ResourceType = typeof(Res))]
        public System.DateTime Created { get; set; }

        [Display(Name = nameof(Res.LevelLabel), ResourceType = typeof(Res))]
        public string Level { get; set; }

        [Display(Name = nameof(Res.MessageLabel), ResourceType = typeof(Res))]
        public string Message { get; set; }

        [Display(Name = nameof(Res.ExceptionLabel), ResourceType = typeof(Res))]
        public string Exception { get; set; }

        [Display(Name = nameof(Res.ApplicationNameLabel), ResourceType = typeof(Res))]
        public string ApplicationName { get; set; }
    }
}
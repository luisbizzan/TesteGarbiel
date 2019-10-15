using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Res = Resources.ApplicationLogStrings;

namespace FWLog.Web.Backoffice.Models.ApplicationLogCtx
{
    public class ApplicationLogListViewModel
    {
        public ApplicationLogListItemViewModel EmptyItem { get; }

        public ApplicationLogFilterViewModel Filter { get; set; }

        public ApplicationLogListViewModel()
        {
            EmptyItem = new ApplicationLogListItemViewModel();
            Filter = new ApplicationLogFilterViewModel();
        }
    }

    public class ApplicationLogFilterViewModel
    {
        [Display(Name = nameof(Res.MessageLabel), ResourceType = typeof(Res))]
        public string Message { get; set; }

        [Display(Name = nameof(Res.LevelLabel), ResourceType = typeof(Res))]
        public string Level { get; set; }

        [Display(Name = nameof(Res.CreatedStartLabel), ResourceType = typeof(Res))]
        public System.DateTime? CreatedStart { get; set; }

        [Display(Name = nameof(Res.CreatedEndLabel), ResourceType = typeof(Res))]
        public System.DateTime? CreatedEnd { get; set; }

        public SelectList LevelOptions { get; set; }
    }

    public class ApplicationLogListItemViewModel
    {
        [Display(Name = nameof(Res.IdApplicationLogLabel), ResourceType = typeof(Res))]
        public int IdApplicationLog { get; set; }

        [Display(Name = nameof(Res.CreatedLabel), ResourceType = typeof(Res))]
        public System.DateTime Created { get; set; }

        [Display(Name = nameof(Res.LevelLabel), ResourceType = typeof(Res))]
        public string Level { get; set; }

        [Display(Name = nameof(Res.MessageLabel), ResourceType = typeof(Res))]
        public string Message { get; set; }

        [Display(Name = nameof(Res.ApplicationNameLabel), ResourceType = typeof(Res))]
        public string ApplicationName { get; set; }
    }
}
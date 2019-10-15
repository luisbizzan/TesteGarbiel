using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Res = Resources.BOGroupStrings;

namespace FWLog.Web.Backoffice.Models.BOGroupCtx
{
    public class BOGroupListViewModel
    {
        public BOGroupListItemViewModel EmptyItem { get; set; }

        public BOGroupFilterViewModel Filter { get; set; }

        public BOGroupListViewModel()
        {
            EmptyItem = new BOGroupListItemViewModel();
            Filter = new BOGroupFilterViewModel();
        }
    }

    public class BOGroupListItemViewModel
    {
        public string Id { get; set; }

        [Display(Name = nameof(Res.NameLabel), ResourceType = typeof(Res))]
        public string Name { get; set; }
    }

    public class BOGroupFilterViewModel
    {
        [Display(Name = nameof(Res.NameLabel), ResourceType = typeof(Res))]
        public string Name { get; set; }
    }
}
﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Res = Resources.BOAccountStrings;

namespace FWLog.Web.Backoffice.Models.BOAccountCtx
{
    public class BOAccountListViewModel
    {
        public BOAccountListItemViewModel EmptyItem { get; private set; }

        public BOAccountFilterViewModel Filter { get; set; }

        public BOAccountListViewModel()
        {
            EmptyItem = new BOAccountListItemViewModel();
            Filter = new BOAccountFilterViewModel();
        }
    }

    public class BOAccountListItemViewModel
    {
        [Display(Name = nameof(Res.UserNameLabel), ResourceType = typeof(Res))]
        public string UserName { get; set; }

        [Display(Name = nameof(Res.EmailLabel), ResourceType = typeof(Res))]
        public string Email { get; set; }

        [Display(Name = nameof(Res.CreationDateLabel), ResourceType = typeof(Res))]
        public DateTime CreationDate { get; set; }
    }

    public class BOAccountFilterViewModel
    {
        [Display(Name = nameof(Res.UserNameLabel), ResourceType = typeof(Res))]
        public string UserName { get; set; }

        [Display(Name = nameof(Res.EmailLabel), ResourceType = typeof(Res))]
        public string Email { get; set; }
    }
}
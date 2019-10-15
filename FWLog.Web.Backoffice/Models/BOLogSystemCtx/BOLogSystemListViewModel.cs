using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Res = Resources.BOLogSystemStrings;

namespace FWLog.Web.Backoffice.Models.BOLogSystemCtx
{
    public class BOLogSystemListViewModel
    {
        public BOLogSystemListItemViewModel EmptyItem { get; }
        public BOLogSystemFilterViewModel Filter { get; set; }

        public BOLogSystemListViewModel()
        {
            EmptyItem = new BOLogSystemListItemViewModel();
            Filter = new BOLogSystemFilterViewModel();
        }
    }

    public class BOLogSystemListItemViewModel
    {
        public long IdBOLogSystem { get; set; }

        [Display(Name = nameof(Res.UserNameLabel), ResourceType = typeof(Res))]
        public string UserName { get; set; }

        [Display(Name = nameof(Res.ActionTypeLabel), ResourceType = typeof(Res))]
        public string ActionType { get; set; }

        [Display(Name = nameof(Res.DescriptionTypeLabel), ResourceType = typeof(Res))]
        public string Description { get; set; }

        [Display(Name = nameof(Res.IPLabel), ResourceType = typeof(Res))]
        public string IP { get; set; }

        [Display(Name = nameof(Res.ExecutionDateLabel), ResourceType = typeof(Res))]
        public System.DateTime ExecutionDate { get; set; }

        [Display(Name = nameof(Res.EntityLabel), ResourceType = typeof(Res))]
        public string Entity { get; set; }

    }

    public class BOLogSystemFilterViewModel
    {
        [Display(Name = nameof(Res.UserNameLabel), ResourceType = typeof(Res))]
        public string UserName { get; set; }

        [Display(Name = nameof(Res.ActionTypeLabel), ResourceType = typeof(Res))]
        public string ActionType { get; set; }

        [Display(Name = nameof(Res.EntityLabel), ResourceType = typeof(Res))]
        public string Entity { get; set; }

        [Display(Name = nameof(Res.IPLabel), ResourceType = typeof(Res))]
        public string IP { get; set; }

        [Display(Name = nameof(Res.ExecutionDateStartLabel), ResourceType = typeof(Res))]
        public DateTime? ExecutionDateStart { get; set; }

        [Display(Name = nameof(Res.ExecutionDateEndLabel), ResourceType = typeof(Res))]
        public DateTime? ExecutionDateEnd { get; set; }

        public SelectList EntityOptions { get; set; }

        public SelectList ActionTypeOptions { get; set; }
    }
}
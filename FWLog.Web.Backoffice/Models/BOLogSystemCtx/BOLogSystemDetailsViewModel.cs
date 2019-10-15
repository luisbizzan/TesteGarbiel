using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Res = Resources.BOLogSystemStrings;

namespace FWLog.Web.Backoffice.Models.BOLogSystemCtx
{
    public class BOLogSystemDetailsViewModel
    {
        [Display(Name = nameof(Res.IdBOLogSistemLabel), ResourceType = typeof(Res))]
        public long IdBOLogSystem { get; set; }

        [Display(Name = nameof(Res.UserNameLabel), ResourceType = typeof(Res))]
        public string UserName { get; set; }

        [Display(Name = nameof(Res.ActionTypeLabel), ResourceType = typeof(Res))]
        public string ActionType { get; set; }

        [Display(Name = nameof(Res.ActionTypeLabel), ResourceType = typeof(Res))]
        public string TranslatedActionType { get; set; }

        [Display(Name = nameof(Res.DescriptionTypeLabel), ResourceType = typeof(Res))]
        public string Description { get; set; }

        [Display(Name = nameof(Res.IPLabel), ResourceType = typeof(Res))]
        public string IP { get; set; }

        [Display(Name = nameof(Res.ExecutionDateLabel), ResourceType = typeof(Res))]
        public System.DateTime ExecutionDate { get; set; }

        [Display(Name = nameof(Res.EntityLabel), ResourceType = typeof(Res))]
        public string Entity { get; set; }

        [Display(Name = nameof(Res.EntityLabel), ResourceType = typeof(Res))]
        public string TranslatedEntity { get; set; }

        [Display(Name = nameof(Res.EntityLabel), ResourceType = typeof(Res))]
        public string EntityDisplay { get => String.Format("{0} ({1})", TranslatedEntity, Entity); }

        public IEnumerable<BOLogSystemColumnChangesViewModel> ColumnChanges { get; set; }
        public IEnumerable<BOLogSystemRelatedViewModel> RelatedLogs { get; set; }
    }

    public class BOLogSystemRelatedViewModel
    {
        public long IdBOLogSystem { get; set; }
        public string Entity { get; set; }
        public string TranslatedEntity { get; set; }
        public string ActionType { get; set; }
    }
}
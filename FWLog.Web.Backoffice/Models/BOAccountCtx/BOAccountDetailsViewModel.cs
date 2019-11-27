using DartDigital.Library.Web.ModelValidation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Res = Resources.BOAccountStrings;

namespace FWLog.Web.Backoffice.Models.BOAccountCtx
{
    public class BOAccountDetailsViewModel
    {
        [Display(Name = nameof(Res.UserNameLabel), ResourceType = typeof(Res))]
        public string UserName { get; set; }

        [Required]
        [MvcEmailValidation]
        [Display(Name = nameof(Res.EmailLabel), ResourceType = typeof(Res))]
        public string Email { get; set; }

        [Display(Name = nameof(Res.DisabledLabel), ResourceType = typeof(Res))]
        public bool Disabled { get; set; }

        public List<GroupItemViewModel> Groups { get; set; }
    }
}
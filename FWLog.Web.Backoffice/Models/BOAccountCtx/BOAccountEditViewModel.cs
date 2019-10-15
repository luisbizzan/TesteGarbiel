using DartDigital.Library.Web.ModelValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Res = Resources.BOAccountStrings;

namespace FWLog.Web.Backoffice.Models.BOAccountCtx
{
    public class BOAccountEditViewModel
    {
        [Display(Name = nameof(Res.UserNameLabel), ResourceType = typeof(Res))]
        public string UserName { get; set; }

        [Required]
        [MvcEmailValidation]
        [Display(Name = nameof(Res.EmailLabel), ResourceType = typeof(Res))]
        public string Email { get; set; }

        public List<GroupItemViewModel> Groups { get; set; }

        public BOAccountEditViewModel()
        {
            Groups = new List<GroupItemViewModel>();
        }
    }
}
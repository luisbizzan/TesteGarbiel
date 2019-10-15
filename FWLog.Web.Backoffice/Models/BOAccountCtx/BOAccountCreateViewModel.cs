using DartDigital.Library.Web.ModelValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Res = Resources.BOAccountStrings;

namespace FWLog.Web.Backoffice.Models.BOAccountCtx
{
    public class BOAccountCreateViewModel
    {
        [Required]
        [Display(Name = nameof(Res.UserNameLabel), ResourceType = typeof(Res))]
        public string UserName { get; set; }

        [Required]
        [MvcEmailValidation]
        [Display(Name = nameof(Res.EmailLabel), ResourceType = typeof(Res))]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = nameof(Res.PasswordLabel), ResourceType = typeof(Res))]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = nameof(Res.ConfirmPasswordLabel), ResourceType = typeof(Res))]
        [Compare("Password", ErrorMessageResourceName = nameof(Res.ConfirmPasswordCompareMessage), ErrorMessageResourceType = typeof(Res))]
        public string ConfirmPassword { get; set; }

        public List<GroupItemViewModel> Groups { get; set; } = new List<GroupItemViewModel>();
    }
}
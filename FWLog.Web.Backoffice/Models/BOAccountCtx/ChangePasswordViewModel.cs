using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Res = Resources.BOAccountStrings;

namespace FWLog.Web.Backoffice.Models.BOAccountCtx
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = nameof(Res.OldPasswordLabel), ResourceType = typeof(Res))]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = nameof(Res.NewPasswordLabel), ResourceType = typeof(Res))]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = nameof(Res.ConfirmNewPasswordLabel), ResourceType = typeof(Res))]
        [Compare("NewPassword", ErrorMessageResourceName = nameof(Res.ConfirmNewPasswordCompareMessage), ErrorMessageResourceType = typeof(Res))]
        public string ConfirmPassword { get; set; }
    }
}
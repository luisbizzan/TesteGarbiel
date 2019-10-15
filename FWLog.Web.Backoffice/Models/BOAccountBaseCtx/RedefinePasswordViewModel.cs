using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Res = Resources.BOAccountBaseStrings;

namespace FWLog.Web.Backoffice.Models.BOAccountBaseCtx
{
    public class RedefinePasswordViewModel
    {
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

        [Required]
        public string Token { get; set; }

        public string UserId { get; set; }

        public string SuccessMessage { get; set; }
    }
}
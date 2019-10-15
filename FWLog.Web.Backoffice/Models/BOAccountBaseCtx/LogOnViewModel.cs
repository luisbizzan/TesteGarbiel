using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Res = Resources.BOAccountBaseStrings;

namespace FWLog.Web.Backoffice.Models.BOAccountBaseCtx
{
    public class LogOnViewModel
    {
        [Required]
        [Display(Name = nameof(Res.UserLabel), ResourceType = typeof(Res))]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = nameof(Res.PasswordLabel), ResourceType = typeof(Res))]
        public string Password { get; set; }

        public string CurrentLanguage { get; set; }

        public SelectList LanguageSelectList { get; set; }

        public string ErrorMessage { get; set; }

        public bool CanChangeLanguage
        {
            get
            {
                if (LanguageSelectList == null)
                    return false;

                return LanguageSelectList.Count() > 1;
            }
        }
    }
}
using DartDigital.Library.Web.ModelValidation;
using System.ComponentModel.DataAnnotations;
using Res = Resources.BOAccountBaseStrings;

namespace FWLog.Web.Backoffice.Models.BOAccountBaseCtx
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [MvcEmailValidation]
        [DataType(DataType.EmailAddress)]
        [Display(Name = nameof(Res.EmailLabel), ResourceType = typeof(Res))]
        public string Email { get; set; }

        public string SuccessMessage { get; set; }
    }
}
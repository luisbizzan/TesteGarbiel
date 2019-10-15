using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FWLog.Web.Backoffice.Models.ExampleCtx
{
    public class AutoCompleteViewModel
    {
        [Required]
        [Display(Name = "Mensagem de Erro")]
        public string AutoCompleteDisplay { get; set; }

        [Required]
        [Display(Name = "Mensagem de Erro")]
        public string AutoCompleteSelectedValue { get; set; }
    }
}
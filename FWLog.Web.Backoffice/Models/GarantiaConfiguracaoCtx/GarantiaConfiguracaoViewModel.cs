using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.GarantiaConfiguracaoCtx
{
    public class GarantiaConfiguracaoViewModel
    {
        [Display(Name = "Codigo")]
        public long Id { get; set; }

        [Required]
        [Display(Name = "Código Fornecedor")]
        public string Cod_Fornecedor { get; set; }
    }
}
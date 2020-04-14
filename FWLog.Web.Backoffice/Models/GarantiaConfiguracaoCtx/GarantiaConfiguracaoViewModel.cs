using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.GarantiaConfiguracaoCtx
{
    public class GarantiaConfiguracaoViewModel
    {
        [Display(Name = "Codigo")]
        public long Id { get; set; }

        [Display(Name = "Consultar Código Fornecedor")]
        public string Cod_Fornecedor { get; set; }

        [Required]
        [Display(Name = "Lista Códigos")]
        public string[] Codigos { get; set; }

        [Display(Name = "Nome Fantasia")]
        public string NomeFantasia { get; set; }

        [Display(Name = "Razão Social")]
        public string RazaoSocial { get; set; }

        [Display(Name = "")]
        public string BotaoEvento { get; set; }
    }
}
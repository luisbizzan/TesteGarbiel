using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.GarantiaConfiguracaoCtx
{
    public class GarantiaConfiguracaoViewModel
    {
        [Required]
        [Display(Name = "TAG")]
        public string Tag { get; set; }

        [Required]
        [Display(Name = "Registro Inclusão")]
        public List<object> Inclusao { get; set; }

        [Display(Name = "Codigo")]
        public long Id { get; set; }

        [Display(Name = "Botão Grid")]
        public string BotaoEvento { get; set; }
    }
}
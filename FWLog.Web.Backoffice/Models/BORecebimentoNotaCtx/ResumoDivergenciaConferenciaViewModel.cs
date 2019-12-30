using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class ResumoDivergenciaConferenciaViewModel
    {
        public ResumoDivergenciaConferenciaViewModel()
        {
            Divergencias = new List<ResumoDivergenciaConferenciaItemViewModel>();
        }

        public long IdNotaFiscal { get; set; }

        public List<ResumoDivergenciaConferenciaItemViewModel> Divergencias { get; set; }
    }

    public class ResumoDivergenciaConferenciaItemViewModel
    {
        [Display(Name = "Referência")]
        public string Referencia { get; set; }
        [Display(Name = "Qtd. Nota Fiscal")]
        public int QuantidadeNotaFiscal { get; set; }
        [Display(Name = "Qtd. Conferência")]
        public int QuantidadeConferencia { get; set; }
        [Display(Name = "A+")]
        public int QuantidadeMais { get; set; }
        [Display(Name = "A-")]
        public int QuantidadeMenos { get; set; }
        [Display(Name = "Qtd. Tratado A+")]
        public int? QuantidadeMaisTratado { get; set; }
        [Display(Name = "Qtd. Tratado A-")]
        public int? QuantidadeMenosTratado { get; set; }
    }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class TratarDivergenciaRecebimentoViewModel
    {
        public TratarDivergenciaRecebimentoViewModel()
        {
            Divergencias = new List<TratarDivergenciaRecebimentoItemViewModel>();
        }

        public long IdNotaFiscal { get; set; }
        [Display(Name = "Nota Fiscal")]
        public string NotaFiscal { get; set; }
        [Display(Name = "Status")]
        public string StatusNotasFiscal { get; set; }
        [Display(Name = "Conferido por")]
        public string ConferidoPor { get; set; }
        [Display(Name = "Início da Conferência")]
        public string InicioConferencia { get; set; }
        [Display(Name = "Fim da Conferência")]
        public string FimConferencia { get; set; }

        [Required]
        [Display(Name = "Observação")]
        public string ObservacaoDivergencia { get; set; }

        public List<TratarDivergenciaRecebimentoItemViewModel> Divergencias { get; set; }
    }

    public class TratarDivergenciaRecebimentoItemViewModel
    {
        [Required]
        public long IdLoteDivergencia { get; set; }
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
        [Display(Name = "Qtd. A+")]
        public int? QuantidadeMaisTratado { get; set; }
        [Display(Name = "Qtd. A-")]
        public int? QuantidadeMenosTratado { get; set; }
    }
}
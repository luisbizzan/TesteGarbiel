using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class ExibirDivergenciaRecebimentoViewModel
    {
        public ExibirDivergenciaRecebimentoViewModel()
        {
            Divergencias = new List<ExibirDivergenciaRecebimentoItemViewModel>();
        }

        public long IdLote { get; set; }
        public string NotaFiscal { get; set; }
        [Display(Name = "Status")]
        public string StatusNotasFiscal { get; set; }
        [Display(Name = "Conferido por")]
        public string ConferidoPor { get; set; }
        [Display(Name = "Início da Conferência")]
        public string InicioConferencia { get; set; }
        [Display(Name = "Fim da Conferência")]
        public string FimConferencia { get; set; }
        [Display(Name = "Tratado por")]
        public string UsuarioTratamento { get; set; }
        [Display(Name = "Data Tratamento")]
        public string DataTratamento { get; set; }

        public List<ExibirDivergenciaRecebimentoItemViewModel> Divergencias { get; set; }
    }

    public class ExibirDivergenciaRecebimentoItemViewModel
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
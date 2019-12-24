using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class ResumoFinalizarConferenciaViewModel
    {
        public ResumoFinalizarConferenciaViewModel()
        {
            Itens = new List<ResumoFinalizarConferenciaItemViewModel>();
        }

        [Display(Name = "Lote")]
        public long IdLote { get; set; }
        [Display(Name = "Nota Fiscal")]
        public int NumeroNotaFiscal { get; set; }
        [Display(Name = "Recebido em")]
        public string DataRecebimento { get; set; }
        [Display(Name = "Fornecedor")]
        public string RazaoSocialFornecedor { get; set; }
        [Display(Name = "Qtde. Volumes")]
        public int QuantidadeVolumes { get; set; }
        [Display(Name = "Tipo de Conferência")]
        public string TipoConferencia { get; set; }
        public long IdNotaFiscal { get; set; }

        public List<ResumoFinalizarConferenciaItemViewModel> Itens { get; set; }
    }

    public class ResumoFinalizarConferenciaItemViewModel
    {
        [Display(Name = "Referência")]
        public string Referencia { get; set; }
        [Display(Name = "Qtde. Nota")]
        public int QuantidadeNota { get; set; }
        [Display(Name = "Qtde. Conferido")]
        public int QuantidadeConferido { get; set; }
        [Display(Name = "Qtde. A+")]
        public int DivergenciaMais { get; set; }
        [Display(Name = "Qtde. A-")]
        public int DivergenciaMenos { get; set; }
    }
}
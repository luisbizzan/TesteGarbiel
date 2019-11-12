using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class BORecebimentoNotaListViewModel
    {
        public BORecebimentoNotaListItemViewModel EmptyItem { get; set; }

        public BORecebimentoNotaFilterViewModel Filter { get; set; }

        public BORecebimentoNotaListViewModel()
        {
            EmptyItem = new BORecebimentoNotaListItemViewModel();
            Filter = new BORecebimentoNotaFilterViewModel();
        }
    }

    public class BORecebimentoNotaListItemViewModel
    {
        public int? Lote { get; set; }

        public int? Nota { get; set; }

        [Display(Name = "Qtde. Peça")]
        public int? QuantidadePeca { get; set; }

        [Display(Name = "Qtde. Volume")]
        public int? QuantidadeVolume { get; set; }

        public int? Atraso { get; set; }

        public string Prazo { get; set; }

        public string Fornecedor { get; set; }

        public string Status { get; set; }
    }

    public class BORecebimentoNotaFilterViewModel
    {
        public string DANFE { get; set; }

        public int? Lote { get; set; }

        public int? Nota { get; set; }

        [Display(Name = "Prazo Recebimento (dias)")]
        public int? Prazo { get; set; }
        
        [Display(Name = "Status")]
        public int? IdStatus { get; set; }

        [Display(Name = "Data Inicial (Entrada)")]
        public DateTime? DataInicial { get; set; }

        [Display(Name = "Data Final (Entrada)")]
        public DateTime? DataFinal { get; set; }

        [Display(Name = "Prazo Inicial")]
        public DateTime? PrazoInicial { get; set; }

        [Display(Name = "Prazo Final")]
        public DateTime? PrazoFinal { get; set; }

        [Display(Name = "Atraso (dias)")]
        public int? Atraso { get; set; }

        [Display(Name = "Quantidade de Peças")]
        public int? QuantidadePeca { get; set; }

        [Display(Name = "Quantidade de Volumes")]
        public int? Volume { get; set; }

        [Display(Name = "Fornecedor")]
        public int? IdFornecedor { get; set; }

        public string RazaoSocialFornecedor { get; set; }

        [Display(Name = "Recebido por")]
        public int? IdUsuarioRecebimento { get; set; }

        public string UserNameRecebimento { get; set; }

        [Display(Name = "Conferido por")]
        public int? IdUsuarioConferencia { get; set; }

        public string UserNameConferencia { get; set; }

        [Display(Name = "Tempo Inicial (Conferência)")]
        public TimeSpan? TempoInicial { get; set; }

        [Display(Name = "Tempo Final (Conferência)")]
        public TimeSpan? TempoFinal { get; set; }

        public SelectList ListaStatus { get; set; }

    }

}
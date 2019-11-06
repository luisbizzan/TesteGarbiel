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
        public string Lote { get; set; }

        public string Nota { get; set; }

        [Display(Name = "Qtde. Peça")]
        public int QuantidadePeca { get; set; }

        [Display(Name = "Qtde. Volume")]
        public int QuantidadeVolume { get; set; }

        public int Atraso { get; set; }

        public string Prazo { get; set; }

        public string Fornecedor { get; set; }

        public string Status { get; set; }
    }

    public class BORecebimentoNotaFilterViewModel
    {
        public string Lote { get; set; }

        public string Nota { get; set; }

        [Display(Name = "Prazo Recebimento (dias)")]
        public string Prazo { get; set; }
        
        public string DANFE { get; set; }

        [Display(Name = "Status")]
        public List<int> IdStatus { get; set; }

        [Display(Name = "Data Inicial (Entrada)")]
        public DateTime DataInicial { get; set; }

        [Display(Name = "Data Final (Entrada)")]
        public DateTime DataFinal { get; set; }

        [Display(Name = "Prazo Inicial")]
        public DateTime PrazoInicial { get; set; }

        [Display(Name = "Prazo Final")]
        public DateTime PrazoFinal { get; set; }

        [Display(Name = "Atraso (dias)")]
        public int Atraso { get; set; }

        [Display(Name = "Quantidade de Peças")]
        public int QuantidadePeca { get; set; }

        [Display(Name = "Quantidade de Volumes")]
        public int Volume { get; set; }

        public string Fornecedor { get; set; }

        [Display(Name = "Recebido por")]
        public string RecebidoPor { get; set; }

        [Display(Name = "Conferido por")]
        public string ConferidoPor { get; set; }

        [Display(Name = "Tempo Inicial")]
        public TimeSpan TempoInicial { get; set; }

        [Display(Name = "Tempo Final")]
        public TimeSpan TempoFinal { get; set; }

        public SelectList Status { get; set; }

    }

}
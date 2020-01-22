using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class BORecebimentoNotaListViewModel
    {
        public BORecebimentoNotaListViewModel()
        {
            EmptyItem = new BORecebimentoNotaListItemViewModel();
            Filter = new BORecebimentoNotaFilterViewModel();
        }

        public BORecebimentoNotaListItemViewModel EmptyItem { get; set; }
        public BORecebimentoNotaFilterViewModel Filter { get; set; }
    }

    public class BORecebimentoNotaListItemViewModel
    {
        public long? Lote { get; set; }
        public long? Nota { get; set; }
        [Display(Name = "Peças")]
        public int? QuantidadePeca { get; set; }
        [Display(Name = "Volumes")]
        public int? QuantidadeVolume { get; set; }
        [Display(Name = "Recebido")]
        public string RecebidoEm { get; set; }
        public long? Atraso { get; set; }
        public string Prazo { get; set; }
        public string Fornecedor { get; set; }
        public int? IdLoteStatus { get; set; }
        public string Status { get; set; }
        public long IdNotaFiscal { get; set; }
        [Display(Name = "Recebido por")]
        public string IdUsuarioRecebimento { get; set; }
    }

    public class BORecebimentoNotaFilterViewModel
    {
        [Display(Name = "Chave Acesso")]
        public string ChaveAcesso { get; set; }
        public long? Lote { get; set; }
        public long? Nota { get; set; }
        [Display(Name = "Status")]
        public long? IdStatus { get; set; }
        [Display(Name = "Data de Recebimento Inicial")]
        public DateTime? DataInicial { get; set; }
        [Display(Name = "Data de Recebimento Final")]
        public DateTime? DataFinal { get; set; }
        [Display(Name = "Prazo de Entrega Inicial")]
        public DateTime? PrazoInicial { get; set; }
        [Display(Name = "Prazo de Entrega Final")]
        public DateTime? PrazoFinal { get; set; }
        [Display(Name = "Atraso na Entrega (dias)")]
        public int? Atraso { get; set; }
        [Display(Name = "Quantidade de Peças")]
        public int? QuantidadePeca { get; set; }
        [Display(Name = "Quantidade de Volumes")]
        public int? QuantidadeVolume { get; set; }
        [Display(Name = "Fornecedor")]
        public long? IdFornecedor { get; set; }
        public string RazaoSocialFornecedor { get; set; }
        [Display(Name = "Recebido por")]
        public string IdUsuarioRecebimento { get; set; }
        public string UserNameRecebimento { get; set; }
        [Display(Name = "Conferido por")]
        public string IdUsuarioConferencia { get; set; }
        public string UserNameConferencia { get; set; }
        [Display(Name = "Tempo de Conferência Inicial")]
        public string TempoInicial { get; set; }
        [Display(Name = "Tempo de Conferência Final")]
        public string TempoFinal { get; set; }
        public SelectList ListaStatus { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.BOQuarentenaCtx
{
    public class BOQuarentenaListViewModel
    {
        public BOQuarentenaListItemViewModel EmptyItem { get; set; }

        public BOQuarentenaFilterViewModel Filter { get; set; }

        public BOQuarentenaListViewModel()
        {
            EmptyItem = new BOQuarentenaListItemViewModel();
            Filter = new BOQuarentenaFilterViewModel();
        }
    }

    public class BOQuarentenaListItemViewModel
    {
        public long IdQuarentena { get; set; }

        public long Lote { get; set; }

        public long Nota { get; set; }

        public string Fornecedor { get; set; }

        public long? Atraso { get; set; }

        [Display(Name = "Data Abertura")]
        public string DataAbertura { get; set; }

        [Display(Name = "Data Encerramento")]
        public string DataEncerramento { get; set; }

        public string Status { get; set; }
    }

    public class BOQuarentenaFilterViewModel
    {
        public string ChaveAcesso { get; set; }

        public long? Lote { get; set; }

        public long? Nota { get; set; }

        [Display(Name = "Status")]
        public long? IdQuarentenaStatus { get; set; }

        [Display(Name = "Data de Abertura Inicial")]
        public DateTime? DataAberturaInicial { get; set; }

        [Display(Name = "Data de Abertura Final")]
        public DateTime? DataAberturaFinal { get; set; }

        [Display(Name = "Data de Encerramento Inicial")]
        public DateTime? DataEncerramentoInicial { get; set; }

        [Display(Name = "Data de Encerramento Final")]
        public DateTime? DataEncerramentoFinal { get; set; }

        [Display(Name = "Dias em Quarentena")]
        public long? Atraso { get; set; }

        [Display(Name = "Fornecedor")]
        public long? IdFornecedor { get; set; }

        public string RazaoSocialFornecedor { get; set; }

        public SelectList ListaQuarentenaStatus { get; set; }

    }
}
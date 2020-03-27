using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.BOAccountCtx
{
    public class HistoricoDeAcoesViewModel
    {
        public HistoricoDeAcoesViewModel()
        {
            EmptyItem = new HistoricoDeAcoesListItemViewModel();
            Filter = new HistoricoDeAcoesFilterViewModel();
        }

        public HistoricoDeAcoesListItemViewModel EmptyItem { get; set; }
        public HistoricoDeAcoesFilterViewModel Filter { get; set; }
    }

    public class HistoricoDeAcoesListItemViewModel
    {
        [Display(Name = "Usuário")]
        public string IdUsuario { get; set; }
        [Display(Name = "Aplicação")]
        public string ColetorAplicacao { get; set; }
        public int IdColetorAplicacao { get; set; }
        [Display(Name = "Tipo")]
        public string HistoricoColetorTipo { get; set; }
        public int IdHistoricoColetorTipo { get; set; }
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
    }

    public class HistoricoDeAcoesFilterViewModel
    {
       
        [Display(Name = "Aplicação")]
        public int? IdColetorAplicacao { get; set; }
        public SelectList ListaColetorAplicacao { get; set; }
        [Display(Name = "Tipo")]
        public int? IdHistoricoColetorTipo { get; set; }
        public SelectList ListaHistoricoColetorTipo { get; set; }
        [Display(Name = "Data Inicial")]
        public DateTime? DataInicial { get; set; }
        [Display(Name = "Data Final")]
        public DateTime? DataFinal { get; set; }
        [Display(Name = "Usuário")]
        public string IdUsuario { get; set; }
        public string UserNameUsuario { get; set; }
        
    }
}
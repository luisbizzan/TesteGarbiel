using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.HistoricoAcaoUsuarioCtx
{
    public class HistoricoAcaoUsuarioViewModel
    {
        public HistoricoAcaoUsuarioViewModel()
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
        public string Usuario { get; set; }
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
        [Display(Name = "Aplicação")]
        public string ColetorAplicacaoDescricao { get; set; }
        public int IdColetorAplicacao { get; set; }
        [Display(Name = "Tipo")]
        public string HistoricoColetorTipoDescricao { get; set; }
        public int IdHistoricoColetorTipo { get; set; }
        [Display(Name = "Data")]
        public string DataHora { get; set; }
    }

    public class HistoricoDeAcoesFilterViewModel
    {
        [Display(Name = "Aplicação")]
        public int? IdColetorAplicacao { get; set; }
        public SelectList ListaColetorAplicacao { get; set; }
        [Display(Name = "Tipo Histórico")]
        public int? IdHistoricoColetorTipo { get; set; }
        public SelectList ListaHistoricoColetorTipo { get; set; }
        [Required]
        [Display(Name = "Data Inicial")]
        public DateTime? DataInicial { get; set; }
        [Required]
        [Display(Name = "Data Final")]
        public DateTime? DataFinal { get; set; }
        [Display(Name = "Usuário")]
        public string IdUsuario { get; set; }
        public string UserNameUsuario { get; set; }
    }
}
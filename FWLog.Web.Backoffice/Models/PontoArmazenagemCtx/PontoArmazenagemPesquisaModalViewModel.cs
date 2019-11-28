using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.PontoArmazenagemCtx
{
    public class PontoArmazenagemPesquisaModalViewModel
    {
        public PontoArmazenagemPesquisaModalViewModel()
        {
            Itens = new PontoArmazenagemPesquisaModalItemViewModel();
            Filtros = new PontoArmazenagemPesquisaModalFiltroViewModel();
        }

        public PontoArmazenagemPesquisaModalItemViewModel Itens { get; set; }
        public PontoArmazenagemPesquisaModalFiltroViewModel Filtros { get; set; }

        public SelectList Status { get; set; }
    }

    public class PontoArmazenagemPesquisaModalItemViewModel
    {
        [Display(Name = "Código")]
        public int IdPontoArmazenagem { get; set; }
        [Display(Name = "Nível")]
        public string NivelArmazenagem { get; set; }
        [Display(Name = "Ponto")]
        public string PontoArmazenagem { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
    }

    public class PontoArmazenagemPesquisaModalFiltroViewModel
    {
        [Display(Name = "Ponto de Armazenagem")]
        public string Descricao { get; set; }
        [Display(Name = "Status")]
        public int? Status { get; set; }
    }
}
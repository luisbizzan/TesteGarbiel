using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.PontoArmazenagemCtx
{
    public class PontoArmazenagemListaViewModel
    {
        public PontoArmazenagemListaViewModel()
        {
            Itens = new PontoArmazenagemListaItemViewModel();
            Filtros = new PontoArmazenagemListaFilterViewModel();
        }

        public PontoArmazenagemListaItemViewModel Itens { get; set; }
        public PontoArmazenagemListaFilterViewModel Filtros { get; set; }

        public SelectList TiposArmazenagem { get; set; }
        public SelectList TiposMovimentacao { get; set; }
        public SelectList Status { get; set; }
    }

    public class PontoArmazenagemListaItemViewModel
    {
        [Display(Name = "ID")]
        public long IdTipoArmazenagem { get; set; }
        [Display(Name = "Nível")]
        public string NivelArmazenagem { get; set; }
        [Display(Name = "Ponto")]
        public string Descricao { get; set; }
        [Display(Name = "Tipo Armazenagem")]
        public string TipoArmazenagem { get; set; }
        [Display(Name = "Tipo Movimentação")]
        public string TipoMovimentacao { get; set; }
        [Display(Name = "Limite Peso")]
        public string LimitePesoVertical { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
    }

    public class PontoArmazenagemListaFilterViewModel
    {
        [Display(Name = "Nível Armazenagem")]
        public long? IdNivelArmazenagem { get; set; }
        [Display(Name = "Tipo Armazenagem")]
        public int? IdTipoArmazenagem { get; set; }
        [Display(Name = "Tipo Movimentação")]
        public int? IdTipoMovimentacao { get; set; }
        [Display(Name = "Status")]
        public bool? Status { get; set; }
        [Display(Name = "Ponto Armazenagem")]
        public string Descricao { get; set; }

        public string DescricaoNivelArmazenagem { get; set; }
    }
}
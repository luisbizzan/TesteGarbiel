using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.CorredorImpressoraCtx
{
    public class CorredorImpressoraListaViewModel
    {
        public CorredorImpressoraListaItemViewModel Itens { get; set; }
        public CorredorImpressoraListaFilterViewModel Filtros { get; set; }

        public CorredorImpressoraListaViewModel()
        {
            Itens = new CorredorImpressoraListaItemViewModel();
            Filtros = new CorredorImpressoraListaFilterViewModel();
        }
    }

    public class CorredorImpressoraListaItemViewModel
    {
        public long IdGrupoCorredorArmazenagem { get; set; }
        public long IdEmpresa { get; set; }
        [Display(Name = "Corredor Inicial")]
        public string CorredorInicial { get; set; }
        [Display(Name = "Corredor Final")]
        public string CorredorFinal { get; set; }
        [Display(Name = "Ponto de Armazenagem")]
        public string DescricaoPontoArmazenagem { get; set; }
        [Display(Name = "Impressora")]
        public string Impressora { get; set; }
        [Display(Name = "Ativo")]
        public string Status { get; set; }
    }

    public class CorredorImpressoraListaFilterViewModel
    {
        [Display(Name = "Ponto de Armazenagem")]
        public long? IdPontoArmazenagem { get; set; }
        public string DescricaoPontoArmazenagem { get; set; }
        [Display(Name = "Corredor")]
        public int? CorredorInicial { get; set; }
        [Display(Name = "Corredor")]
        public int? CorredorFinal { get; set; }
        [Display(Name = "Impressora")]
        public int? IdImpressora { get; set; }
        public SelectList ListaImpressora { get; set; }
        [Display(Name = "Ativo")]
        public bool? Status { get; set; }
        public SelectList ListaStatus { get; set; }
    }
}
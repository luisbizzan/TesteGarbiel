using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.EnderecoArmazenagemCtx
{
    public class EnderecoArmazenagemListaViewModel
    {
        public EnderecoArmazenagemListaViewModel()
        {
            Itens = new EnderecoArmazenagemListaItemViewModel();
            Filtros = new EnderecoArmazenagemListaFilterViewModel();
        }

        public EnderecoArmazenagemListaItemViewModel Itens { get; set; }
        public EnderecoArmazenagemListaFilterViewModel Filtros { get; set; }
        public SelectList Status { get; set; }
        public SelectList Picking { get; set; }
        public SelectList PontoSeparacao { get; set; }
    }

    public class EnderecoArmazenagemListaFilterViewModel
    {
        [Display(Name = "Nível Armazenagem")]
        public long? IdNivelArmazenagem { get; set; }
        [Display(Name = "Ponto Armazenagem")]
        public long? IdPontoArmazenagem { get; set; }
        [Display(Name = "Status")]
        public int? Status { get; set; }
        [Display(Name = "Código Endereço")]
        public string Codigo { get; set; }
        [Display(Name = "Picking?")]
        public int? Picking { get; set; }
        [Display(Name = "Ponto de Separação?")]
        public int? PontoSeparacao { get; set; }
        public string DescricaoNivelArmazenagem { get; set; }
        public string DescricaoPontoArmazenagem { get; set; }
    }

    public class EnderecoArmazenagemListaItemViewModel
    {
        [Display(Name = "ID")]
        public long IdEnderecoArmazenagem { get; set; }
        [Display(Name = "Nível")]
        public string NivelArmazenagem { get; set; }
        [Display(Name = "Ponto")]
        public string PontoArmazenagem { get; set; }
        [Display(Name = "Código")]
        public string Codigo { get; set; }
        [Display(Name = "FIFO")]
        public string Fifo { get; set; }
        [Display(Name = "Ponto Separação")]
        public string PontoSeparacao { get; set; }
        [Display(Name = "Picking")]
        public string Picking { get; set; }
        [Display(Name = "Estoque Minimo")]
        public int EstoqueMinimo { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
        [Display(Name = "Quantidade")]
        public int Quantidade { get; set; }

        public bool Ocupado { get; set; }
    }
}
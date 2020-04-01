using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.EnderecoArmazenagemCtx
{
    public class EnderecoArmazenagemPesquisaModalViewModel
    {
        public EnderecoArmazenagemPesquisaModalViewModel()
        {
            Itens = new EnderecoArmazenagemPesquisaModalItemViewModel();
            Filtros = new EnderecoArmazenagemPesquisaModalFiltroViewModel();
        }

        public EnderecoArmazenagemPesquisaModalItemViewModel Itens { get; set; }
        public EnderecoArmazenagemPesquisaModalFiltroViewModel Filtros { get; set; }
    }

    public class EnderecoArmazenagemPesquisaModalItemViewModel
    {
        public long IdEnderecoArmazenagem { get; set; }
        [Display(Name = "Código")]
        public string Codigo { get; set; }
        [Display(Name = "Limite de Peso")]
        public string LimitePeso { get; set; }
        [Display(Name = "FIFO")]
        public string Fifo { get; set; }
        [Display(Name = "Estoque Mínimo")]
        public string EstoqueMinimo { get; set; }
        [Display(Name = "Estoque Máximo")]
        public string EstoqueMaximo { get; set; }
    }

    public class EnderecoArmazenagemPesquisaModalFiltroViewModel
    {
        [Display(Name = "Código")]
        public string Codigo { get; set; }
        public long? IdPontoArmazenagem { get; set; }
    }
}
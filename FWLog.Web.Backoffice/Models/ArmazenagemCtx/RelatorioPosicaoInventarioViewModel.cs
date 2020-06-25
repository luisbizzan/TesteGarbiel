using System.ComponentModel.DataAnnotations;


namespace FWLog.Web.Backoffice.Models.ArmazenagemCtx
{
    public class RelatorioPosicaoInventarioViewModel
    {
        public RelatorioPosicaoInventarioListItemViewModel EmptyItem { get; set; }

        public RelatorioPosicaoInventarioFilterViewModel Filter { get; set; }


        public RelatorioPosicaoInventarioViewModel()
        {
            EmptyItem = new RelatorioPosicaoInventarioListItemViewModel();
            Filter = new RelatorioPosicaoInventarioFilterViewModel();
        }
    }

    public class RelatorioPosicaoInventarioListItemViewModel
    {
        [Display(Name = "")]
        public string Referencia { get; set; }

        [Display(Name = "Lote")]
        public string IdLote { get; set; }

        [Display(Name = "Endereço")]
        public string Codigo { get; set; }

        [Display(Name = "Quantidade")]
        public int QuantidadeProdutoPorEndereco { get; set; }
    }

    public class RelatorioPosicaoInventarioFilterViewModel
    {
        [Display(Name = "Nível de Armazenagem")]
        public long? IdNivelArmazenagem { get; set; }
        public string DescricaoNivelArmazenagem { get; set; }

        [Display(Name = "Ponto de Armazenagem")]
        public long? IdPontoArmazenagem { get; set; }
        public string DescricaoPontoArmazenagem { get; set; }

        [Display(Name = "Produto")]
        public long? IdProduto { get; set; }
        public string DescricaoProduto { get; set; }

        [Display(Name = "Corredor Início")]
        public int? CorredorInicial { get; set; }

        [Display(Name = "Corredor Fim")]
        public int? CorredorFinal { get; set; }
    }
}
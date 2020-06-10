using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.ProdutoCtx
{
    public class ProdutoListaViewModel
    {
        public ProdutoListaItemViewModel Itens { get; set; }
        public ProdutoListaFilterViewModel Filtros { get; set; }

        public ProdutoListaViewModel()
        {
            Itens = new ProdutoListaItemViewModel();
            Filtros = new ProdutoListaFilterViewModel();
        }
    }

    public class ProdutoListaItemViewModel
    {
        public long? IdEnderecoArmazenagem { get; set; }
        public long IdProduto { get; set; }
        [Display(Name = "Referência")]
        public string Referencia { get; set; }
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
        [Display(Name = "Peso")]
        public string Peso { get; set; }
        [Display(Name = "Saldo")]
        public string Saldo { get; set; }
        [Display(Name = "Largura")]
        public string Largura { get; set; }
        [Display(Name = "Altura")]
        public string Altura { get; set; }
        [Display(Name = "Comprimento")]
        public string Comprimento{ get; set; }
        [Display(Name = "Unidade")]
        public string Unidade { get; set; }
        [Display(Name = "Múltiplo")]
        public string Multiplo { get; set; }
        [Display(Name = "Endereço")]
        public string Endereco { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
    }

    public class ProdutoListaFilterViewModel
    {
        [Display(Name = "Ponto de Armazenagem")]
        public long? IdPontoArmazenagem { get; set; }
        public string DescricaoPontoArmazenagem { get; set; }
        [Display(Name = "Nível de Armazenagem")]
        public long? IdNivelArmazenagem { get; set; }
        public string DescricaoNivelArmazenagem { get; set; }
        [Display(Name = "Endereço de Armazenagem")]
        public long? IdEnderecoArmazenagem { get; set; }
        public string CodigoEnderecoArmazenagem { get; set; }
        public long IdEmpresa { get; set; }
        [Display(Name = "Referência")]
        public string Referencia { get; set; }
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
        [Display(Name = "Cód. Barras")]
        public string CodigoDeBarras { get; set; }
        [Display(Name = "Status")]
        public int? ProdutoStatus { get; set; }
        [Display(Name = "Locação/Saldo")]
        public int? LocacaoSaldo { get; set; }
    }
}
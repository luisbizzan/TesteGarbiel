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
        public long IdProduto { get; set; }
        [Display(Name = "Referência")]
        public string Referencia { get; set; }
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
        [Display(Name = "Peso")]
        public string Peso { get; set; }
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

        public long IdEmpresa { get; set; }
        [Display(Name = "Referência")]
        public string Referencia { get; set; }
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
        [Display(Name = "Cód. Barras")]
        public string CodigoDeBarras { get; set; }
        [Display(Name = "Status")]
        public string ProdutoStatus { get; set; }
    }
}
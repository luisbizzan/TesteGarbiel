using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.ArmazenagemCtx
{
    public class RelatorioRastreabilidadeLoteProdutoViewModel
    {
        public RelatorioRastreabilidadeLoteProdutoListItemViewModel EmptyItem { get; set; }

        public RelatorioRastreabilidadeLoteProdutoFilterViewModel Filter { get; set; }

        public RelatorioRastreabilidadeLoteProdutoViewModel()
        {
            EmptyItem = new RelatorioRastreabilidadeLoteProdutoListItemViewModel();
            Filter = new RelatorioRastreabilidadeLoteProdutoFilterViewModel();
        }
    }

    public class RelatorioRastreabilidadeLoteProdutoListItemViewModel
    {
        public long IdProduto { get; set; }

        public long IdLote { get; set; }

        [Display(Name = "Referência")]
        public string ReferenciaProduto { get; set; }

        [Display(Name = "Descrição")]
        public string DescricaoProduto { get; set; }

        [Display(Name = "Qtde. Recebido")]
        public string QuantidadeRecebida { get; set; }

        public string Saldo { get; set; }
    }

    public class RelatorioRastreabilidadeLoteProdutoFilterViewModel
    {
        public long IdLote { get; set; }

        public long IdEmpresa { get; set; }

        [Display(Name = "Produto")]
        public string DescricaoProduto { get; set; }

        public long? IdProduto { get; set; }
    }
}
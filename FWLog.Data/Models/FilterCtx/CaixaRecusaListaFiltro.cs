using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models.FilterCtx
{
    public class CaixaRecusaListaFiltro
    {
        public long IdEmpresa { get; set; }

        public long? IdCaixa { get; set; }

        [Display(Name = "Caixa")]
        public string DescricaoCaixa { get; set; }

        public long? IdProduto { get; set; }

        [Display(Name = "Produto")]
        public string DescricaoProduto { get; set; }

    }
}

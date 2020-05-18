using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.ProdutoCtx
{
    public class ProdutoConfirmaImpressaoViewModel
    {
        [Display(Name = "Código")]
        public long IdEnderecoArmazenagem { get; set; }

        [Display(Name = "Endereco Armazenagem")]
        public string Codigo { get; set; }

        public long IdProduto { get; set; }

        public string DescricaoProduto { get; set; }

        public string Referencia { get; set; }

        [Display(Name = "Tipo Impressão")]
        public TipoImpressaoEtiqueta? TipoImpressaoEtiqueta { get; set; }
    }
}
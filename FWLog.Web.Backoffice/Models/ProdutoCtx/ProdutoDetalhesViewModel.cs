using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.ProdutoCtx
{
    public class ProdutoDetalhesViewModel
    {
        public long IdProduto { get; set; }
        [Display(Name = "Referência")]
        public string Referencia { get; set; }
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
        [Display(Name = "Largura")]
        public string Largura { get; set; }
        [Display(Name = "Altura")]
        public string Altura { get; set; }
        [Display(Name = "Comprimento")]
        public string Comprimento { get; set; }
        [Display(Name = "Peso")]
        public string Peso { get; set; }
        public string ImagemSrc { get; set; }
        [Display(Name = "Endereco Armazenagem")]
        public string EnderecoArmazenagem { get; set; }
    }
}
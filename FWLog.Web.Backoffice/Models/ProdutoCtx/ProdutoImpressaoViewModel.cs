using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.ProdutoCtx
{
    public class ProdutoImpressaoViewModel
    {
        [Required]
        public long IdImpressora { get; set; }
        
        [Required]
        public long IdEnderecoArmazenagem { get; set; }
       
        [Required]
        public long IdProduto { get; set; }

        [Required]
        public TipoImpressaoEtiqueta TipoImpressaoEtiqueta { get; set; }
    }
}
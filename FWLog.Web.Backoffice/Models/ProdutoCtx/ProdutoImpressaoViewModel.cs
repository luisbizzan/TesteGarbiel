using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
    }
}
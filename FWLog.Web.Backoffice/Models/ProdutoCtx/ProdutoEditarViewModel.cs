﻿using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.ProdutoCtx
{
    public class ProdutoEditarViewModel
    {
        public long IdProduto { get; set; }
        [Required]
        [Display(Name = "Ponto de Armazenagem")]
        public long? IdPontoArmazenagem { get; set; }
        public string DescricaoPontoArmazenagem { get; set; }
        [Required]
        [Display(Name = "Nível de Armazenagem")]
        public long? IdNivelArmazenagem { get; set; }
        public string DescricaoNivelArmazenagem { get; set; }
        [Required]
        [Display(Name = "Endereço de Armazenagem")]
        public long? IdEnderecoArmazenagem { get; set; }
        public string CodigoEnderecoArmazenagem { get; set; }
    }
}
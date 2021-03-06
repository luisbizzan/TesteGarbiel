﻿using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Armazenagem
{
    public class ValidarEnderecoInstalacaoModelRequisicao
    {
        [Required(ErrorMessage = "O lote deve ser informado.")]
        public long IdLote { get; set; }
        [Required(ErrorMessage = "O produto deve ser informado.")]
        public long IdProduto { get; set; }
        [Required(ErrorMessage = "A quantidade deve ser informada.")]
        public int Quantidade { get; set; }
        [Required(ErrorMessage = "O endereço deve ser informado.")]
        public long IdEnderecoArmazenagem { get; set; }
    }
}
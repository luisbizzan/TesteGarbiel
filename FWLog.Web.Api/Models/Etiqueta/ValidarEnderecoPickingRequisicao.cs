﻿using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Etiqueta
{
    public class ValidarEnderecoPickingRequisicao
    {
        [Required(ErrorMessage = "O endereço deve ser informado.")]
        public long IdEnderecoArmazenagem { get; set; }
    }
}
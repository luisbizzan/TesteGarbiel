﻿using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Expedicao
{
    public class ImprimirRomaneioRequisicao
    {
        [Required]
        public long IdRomaneio { get; set; }

        [Required]
        public long IdImpressora { get; set; }

        [Required]
        public bool ImprimeSegundaVia { get; set; }
    }
}
﻿using System;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.HistoricoAcaoUsuarioCtx
{
    public class ImprimirHistoricoAcaoUsuarioViewModel
    {
        public int? IdColetorAplicacao { get; set; }
        public int? IdHistoricoColetorTipo { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public string IdUsuario { get; set; }
        public string ColetorAplicacao { get; set; }
        public string HistoricoColetorTipo { get; set; }
        public string UsuarioSelecionado { get; set; }
        [Required]
        public int IdImpressora { get; set; }
    }
}

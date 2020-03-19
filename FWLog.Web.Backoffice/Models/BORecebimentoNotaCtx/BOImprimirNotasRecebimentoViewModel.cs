using System;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class BOImprimirNotasRecebimentoViewModel
    {
        public long IdImpressora { get; set; }
        public string NumeroNF { get; set; }
        public string ChaveAcesso { get; set; }
        public string Serie { get; set; }
        public string Valor { get; set; }
        public long? IdStatus { get; set; }
        public string Status { get; set; }
        public DateTime? DataRegistroInicial { get; set; }
        public DateTime? DataRegistroFinal { get; set; }
        public DateTime? DataSincronismoInicial { get; set; }
        public DateTime? DataSincronismoFinal { get; set; }
        public int? IdFornecedor { get; set; }
        public int? DiasAguardando { get; set; }
        public int? QuantidadeVolumes { get; set; }
        public string IdUsuarioRecebimento { get; set; }
        public string NomeUsuario { get; set; }
    }
}
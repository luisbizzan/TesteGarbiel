using System;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class BOImprimirRelatorioNotasViewModel
    {
        public string Lote { get; set; }
        public string Nota { get; set; }
        public string Prazo { get; set; }
        public string DANFE { get; set; }
        public int? IdStatus { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public DateTime? PrazoInicial { get; set; }
        public DateTime? PrazoFinal { get; set; }
        public int? Atraso { get; set; }
        public int? QuantidadePeca { get; set; }
        public int? Volume { get; set; }
        public int? IdFornecedor { get; set; }
        public int? RecebidoPor { get; set; }
        public string ConferidoPor { get; set; }
        public TimeSpan? TempoInicial { get; set; }
        public TimeSpan? TempoFinal { get; set; }
        [Required]
        public int IdImpressora { get; set; }
    }
}
using FWLog.Data.Models;
using System;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class BONotaRecebimentoViewModel 
    {
        public long     IdNotaFiscalRecebimento { get; set; }   
        public long     IdFornecedor            { get; set; }
        public int      NumeroNF                { get; set; }
        public string   Serie                   { get; set; }
        public decimal  Valor                   { get; set; }
        public int      QuantidadeVolumes       { get; set; }
        public DateTime DataHora                { get; set; }
        public StatusNotaRecebimentoEnum Status { get; set; }

    }
}
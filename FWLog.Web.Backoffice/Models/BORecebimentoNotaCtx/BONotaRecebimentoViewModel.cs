using FWLog.Data.Models;
using System;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class BONotaRecebimentoViewModel 
    {
        public long     IdNotaFiscalRecebimento { get; set; }   
        public long     IdFornecedor            { get; set; }
        public string   FornecedorNome          { get; set; }
        public int      NumeroNF                { get; set; }
        public string   Serie                   { get; set; }
        public string   NumeroSerieNotaFiscal   { get; set; }
        public string   Valor                   { get; set; }
        public int      QuantidadeVolumes       { get; set; }
        public string   HoraRecebimento         { get; set; }
        public string   DataRecebimento         { get; set; }
        public DateTime DataHora                { get; set; }
        public StatusNotaRecebimentoEnum Status { get; set; }
        public string   ChaveAcesso             { get; set; }

    }
}
using System;

namespace FWLog.Services.Model.IntegracaoLog
{
    public class IntegracaoLog
    {
        public long     IdIntegracaoLog        { get; set; }
        public long     IdEmpresa              { get; set; }
        public int      IdIntegracaoTipo       { get; set; }
        public int      IdIntegracaoEntidade   { get; set; }
        public DateTime DataRequisicao         { get; set; }
        public string   HttpVerbo              { get; set; }
        public string   Url                    { get; set; }
        public string   CabecalhoRequisicao    { get; set; }
        public string   CorpoRequisicao        { get; set; }
        public int      Status                 { get; set; }
        public TimeSpan Duracao                { get; set; }
        public string   CabecalhoResposta      { get; set; }
        public string   CorpoResposta          { get; set; }

    }
}

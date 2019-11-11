using FWLog.Services.Integracao.Helpers;
using System;

namespace FWLog.Services.Model
{
    [TabelaIntegracao(DisplayName = "TGFITE")]
    public class NotaFiscalItemIntegracao
    {
        public string NUNOTA { get; set; }
        public string CODPROD { get; set; }
        public string CODVOL { get; set; }
        public string QTDNEG { get; set; }
        public string VLRUNIT { get; set; }
        public string VLRTOT { get; set; }
    }
}

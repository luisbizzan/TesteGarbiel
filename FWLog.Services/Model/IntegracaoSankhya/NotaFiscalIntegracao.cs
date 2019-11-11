using FWLog.Services.Integracao.Helpers;
using System;

namespace FWLog.Services.Model
{
    [TabelaIntegracao(DisplayName = "TGFCAB")]
    public class NotaFiscalIntegracao
    {
        public string NUNOTA { get; set; }
        public string NUMNOTA { get; set; }
        public string SERIENOTA { get; set; }
        public string CODPARCTRANSP { get; set; }
        public string CHAVENFE { get; set; }
        public string DANFE { get; set; }
        public string VLRNOTA { get; set; }
        public string CIF_FOB { get; set; }
        public string CODEMP { get; set; }
        public string STATUSNOTA { get; set; }
        public string VLRFRETE { get; set; }
        public string PESOBRUTO { get; set; }
        public string QTDVOL { get; set; }
        public string NUMCF { get; set; }
    }
}

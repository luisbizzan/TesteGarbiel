using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class BODetalhesEtiquetaConferenciaViewModel
    {
        public string NomeFornecedor { get; set; }
        public string NumeroNotaFiscal { get; set; }
        public string DataHoraRecebimento { get; set; }
        public string QuantidadeVolumes { get; set; }
    }
}
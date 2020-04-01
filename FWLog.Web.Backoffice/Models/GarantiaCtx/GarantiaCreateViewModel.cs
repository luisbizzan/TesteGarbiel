using FWLog.Data.Models;
using System;

namespace FWLog.Web.Backoffice.Models.GarantiaCtx
{
    public class GarantiaCreateViewModel
    {
        public long IdGarantia { get; set; }

        public long IdNotaFiscal { get; set; }

        public GarantiaStatusEnum IdGarantiaStatus { get; set; }

        public long IdRepresentante { get; set; }

        public string IdUsuarioRecebimento { get; set; }

        public string ChaveAcesso { get; set; }

        public DateTime DataRecebimento { get; set; }

        public string Observacao { get; set; }

        public string InformacaoTransporte { get; set; }

        public DateTime DataIncioConferencia { get; set; }

        public DateTime DataFimConferencia { get; set; }

    }
}
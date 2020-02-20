using System;

namespace FWLog.Data.Models.FilterCtx
{
    public class GarantiaFilter
    {
        public long IdEmpresa { get; set; }
        public long? IdGarantia { get; set; }
        public long? IdCliente { get; set; }
        public long? IdTransportadora { get; set; }
        public long? NumeroNF { get; set; }
        public string NumeroFicticioNF { get; set; }
        public string ChaveAcesso { get; set; }
        public DateTime? DataEmissaoInicial { get; set; }
        public DateTime? DataEmissaoFinal { get; set; }
        public DateTime? DataRecebimentoInicial { get; set; }
        public DateTime? DataRecebimentoFinal { get; set; }
        public string IdUsuarioRecebimento { get; set; }
        public string IddUsuarioConferente { get; set; }
        public long? IdGarantiaStatus { get; set; }
    }
}


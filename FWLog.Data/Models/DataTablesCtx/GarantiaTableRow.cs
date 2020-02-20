using System;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class GarantiaTableRow
    {
        public long IdEmpresa { get; set; }
        public long? IdGarantia { get; set; }
        public long? IdCliente { get; set; }
        public long? IdTransportadora { get; set; }
        public long? NumeroNF { get; set; }
        public string NumeroFicticioNF { get; set; }
        public string ChaveAcesso { get; set; }
        public DateTime? DataEmissao { get; set; }                
        public DateTime? DataRecebimento { get; set; }
        public string IdUsuarioRecebimento { get; set; }
        public GarantiaStatusEnum IdGarantiaStatus { get; set; }
    }
}

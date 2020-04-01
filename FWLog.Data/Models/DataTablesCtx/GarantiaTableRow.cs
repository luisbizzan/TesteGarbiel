using System;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class GarantiaTableRow
    {
        public long IdEmpresa { get; set; }
        public long IdNotaFiscal { get; set; }
        public long? IdGarantia { get; set; }
        public string Cliente { get; set; }
        public string CNPJCliente { get; set; }
        public string Transportadora { get; set; }
        public string Fornecedor { get; set; }
        public long? NumeroNF { get; set; }
        public string NumeroFicticioNF { get; set; }
        public string ChaveAcesso { get; set; }
        public DateTime? DataEmissao { get; set; }                
        public DateTime? DataRecebimento { get; set; }          
        public string GarantiaStatus { get; set; }
        public int IdGarantiaStatus { get; set; }
    }
}
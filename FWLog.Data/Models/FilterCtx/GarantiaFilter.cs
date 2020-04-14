using System;

namespace FWLog.Data.Models.FilterCtx
{
    public class GarantiaFilter
    {
        public string Cli_Cnpj { get; set; }
        public long? Id { get; set; }
        public string Nota_Fiscal { get; set; }
        public string Serie { get; set; }
        public long? Id_Status { get; set; }
        public DateTime? Data_Inicial { get; set; }
        public DateTime? Data_Final { get; set; }
        public long? Id_Tipo { get; set; }
    }
}
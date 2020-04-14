using System;

namespace FWLog.Data.Models
{
    public class GarantiaConfiguracao
    {
        public long Id { get; set; }
        public string Cod_Fornecedor { get; set; }
        public string[] Codigos { get; set; }
        public string Value { get; set; }
        public string NomeFantasia { get; set; }
        public string RazaoSocial { get; set; }
        public string BotaoEvento { get; set; }
    }
}

using System;

namespace FWLog.Services.Relatorio
{
    public class ColunaRelatorioAttribute : Attribute
    {
        public string Nome { get; set; }
        public int Tamanho { get; set; }
        public bool DataHora { get; set; }
    }
}

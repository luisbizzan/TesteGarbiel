using System;

namespace FWLog.Data.Models
{
    public class GeralTipo
    {
        public long Id { get; set; }
        public string Tabela { get; set; }
        public string Coluna { get; set; }
        public string Descricao { get; set; }
        public DateTime Dt_Cad { get; set; }
    }
}
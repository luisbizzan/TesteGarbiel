using System;

namespace FWLog.Data.Models
{
    public class GeralHistorico
    {
        public long Id { get; set; }
        public long Id_Categoria { get; set; }
        public long Id_Ref { get; set; }
        public string Id_Usr { get; set; }
        public string Historico { get; set; }
        public string Usuario { get; set; }
        public DateTime Dt_Cad { get; set; }
    }

    public class GeralHistoricoCategoria
    {
        public long Id { get; set; }
        public string Tabela { get; set; }
        public string Categoria { get; set; }
    }
}
using System;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class LogEtiquetagemListaLinhaTabela
    {
        public long IdLogEtiquetagem { get; set; }

        public string Referencia { get; set; }

        public string Descricao { get; set; }

        public string TipoEtiquetagem { get; set; }

        public int Quantidade { get; set; }

        public DateTime DataHora { get; set; }

        public string Usuario { get; set; }
    }
}

using System;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class HistoricoAcaoUsuarioLinhaTabela
    {
        public string Usuario { get; set; }
        public string Descricao { get; set; }
        public string ColetorAplicacaoDescricao { get; set; }
        public int IdColetorAplicacao { get; set; }
        public string HistoricoColetorTipoDescricao { get; set; }
        public int IdHistoricoColetorTipo { get; set; }
        public DateTime DataHora { get; set; }
    }
}

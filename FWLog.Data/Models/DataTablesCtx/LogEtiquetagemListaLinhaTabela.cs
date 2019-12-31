namespace FWLog.Data.Models.DataTablesCtx
{
    public class LogEtiquetagemListaLinhaTabela
    {
        public long IdLogEtiquetagem { get; set; }

        public long Referencia { get; set; }

        public string Descricao { get; set; }

        public string TipoEtiquetagem { get; set; }

        public int Quantidade { get; set; }

        public string DataHora { get; set; }

        public string Usuario { get; set; }
    }
}

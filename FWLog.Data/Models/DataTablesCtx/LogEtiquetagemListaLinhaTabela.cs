namespace FWLog.Data.Models.DataTablesCtx
{
    public class LogEtiquetagemListaLinhaTabela
    {
        public long IdLogEtiquetagem { get; set; }

        public long IdProduto { get; set; }

        public string DescricaoProduto { get; set; }

        public string TipoEtiquetagem { get; set; }

        public long Quantidade { get; set; }

        public string DataHora { get; set; }

        public string Usuario { get; set; }
    }
}

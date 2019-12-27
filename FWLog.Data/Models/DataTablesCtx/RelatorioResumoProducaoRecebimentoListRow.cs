namespace FWLog.Data.Models.DataTablesCtx
{
    public class RelatorioResumoProducaoRecebimentoListRow
    {
        public string Nome { get; set; }

        public long NOTASRECEBIDASUSUARIO { get; set; }

        public long VOLUMESRECEBIDOSUSUARIO { get; set; }

        public long NOTASRECEBIDAS { get; set; }

        public long VOLUMESRECEBIDOS { get; set; }

        public decimal PERCENTUAL { get; set; }

        public long RANKING { get; set; }
    }
}

namespace FWLog.Data.Models.DataTablesCtx
{
    public class RelatorioResumoProducaoConferenciaListRow
    {
        public string Nome { get; set; }

        public string UsuarioId { get; set; }

        public long LOTESRECEBIDASUSUARIO { get; set; }

        public long PECASRECEBIDASUSUARIO { get; set; }

        public long LOTESRECEBIDOS { get; set; }

        public long PECASRECEBIDAS { get; set; }

        public decimal PERCENTUAL { get; set; }

        public long RANKING { get; set; }
    }
}

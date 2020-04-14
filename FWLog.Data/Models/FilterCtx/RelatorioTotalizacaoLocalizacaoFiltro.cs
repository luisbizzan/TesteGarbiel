namespace FWLog.Data.Models.FilterCtx
{
    public class RelatorioTotalizacaoLocalizacaoFiltro
    {
        public long IdEmpresa { get; set; }

        public long IdNivelArmazenagem { get; set; }

        public long IdPontoArmazenagem { get; set; }

        public int? CorredorInicial { get; set; }

        public int? CorredorFinal { get; set; }

        public bool ImprimirVazia { get; set; }
    }
}
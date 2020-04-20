using System;

namespace FWLog.Data.Models.FilterCtx
{
    public class RelatorioLogisticaCorredorListaFiltro
    {
        public long IdNivelArmazenagem { get; set; }
        public long IdEmpresa { get; set; }
        public long IdPontoArmazenagem { get; set; }
        public int? CorredorInicial { get; set; }
        public int? CorredorFinal { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public int Ordenacao { get; set; }
    }
}

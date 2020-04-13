using System;

namespace FWLog.Web.Backoffice.Models.ArmazenagemCtx
{
    public class DownloadRelatorioLogisticaCorredorViewModel
    {
        public long IdNivelArmazenagem { get; set; }
        public long IdPontoArmazenagem { get; set; }
        public int? CorredorInicial { get; set; }
        public int? CorredorFinal { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public int Ordenacao { get; set; }
    }
}
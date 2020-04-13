using System;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.ArmazenagemCtx
{
    public class ImprimirRelatorioLogisticaCorredorViewModel
    {
        public long IdNivelArmazenagem { get; set; }
        public long IdPontoArmazenagem { get; set; }
        public int? CorredorInicial { get; set; }
        public int? CorredorFinal { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public int Ordenacao { get; set; }
        [Required]
        public int IdImpressora { get; set; }
    }
}
using System;

namespace FWLog.Services.Model.Relatorios
{
    public class RelatorioLogisticaCorredorRequest
    {
        public long IdEmpresa { get; set; }
        public string NomeUsuarioRequisicao { get; set; }
        public long IdNivelArmazenagem { get; set; }
        public long IdPontoArmazenagem { get; set; }
        public int? CorredorInicial { get; set; }
        public int? CorredorFinal { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public int Ordenacao { get; set; }
    }
}

using System;

namespace FWLog.Web.Api.Models.Lote
{
    public class BuscarLotePorIdResposta
    {
        public long IdLote { get; set; }
        public int IdLoteStatus { get; set; }
        public long IdNotaFiscal { get; set; }
        public DateTime DataRecebimento { get; set; }
        public int QuantidadeVolume { get; set; }
        public int QuantidadePeca { get; set; }
    }
}
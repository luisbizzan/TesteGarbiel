using System;

namespace FWLog.Data.Models
{
    public class Lote
    {
        public int IdLote { get; set; }
        public int IdLoteStatus { get; set; }
        public int IdNotaFiscal { get; set; }
        public DateTime DataCompra { get; set; }
        public DateTime DataRecebimento { get; set; }
        public int QuantidadePeca { get; set; }
        public int QuantidadeVolume { get; set; }
        public string RecebidoPor { get; set; }
        public string ConferidoPor { get; set; }

        public virtual LoteStatus LoteStatus { get; set; }
        public virtual NotaFiscal NotaFiscal { get; set; }
    }
}

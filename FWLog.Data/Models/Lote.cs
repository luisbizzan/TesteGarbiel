using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class Lote
    {
        [Key]
        public long IdLote { get; set; }
        public long IdLoteStatus { get; set; }
        public long IdNotaFiscal { get; set; }
        public DateTime DataCompra { get; set; }
        public DateTime DataRecebimento { get; set; }
        public int QuantidadePeca { get; set; }
        public int QuantidadeVolume { get; set; }
        public string RecebidoPor { get; set; }
        public string ConferidoPor { get; set; }

        [ForeignKey(nameof(IdLoteStatus))]
        public LoteStatus LoteStatus { get; set; }

        [ForeignKey(nameof(IdNotaFiscal))]
        public NotaFiscal NotaFiscal { get; set; }
    }
}

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
        public DateTime? DataRecebimento { get; set; }
        public long QuantidadePeca { get; set; }
        public long QuantidadeVolume { get; set; }
        public string IdUsuarioRecebimento { get; set; }

        [ForeignKey(nameof(IdLoteStatus))]
        public LoteStatus LoteStatus { get; set; }

        [ForeignKey(nameof(IdNotaFiscal))]
        public NotaFiscal NotaFiscal { get; set; }

        public AspNetUsers UsuarioRecebimento { get; set; }
    }
}

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
        public virtual LoteStatus LoteStatus { get; set; }

        [ForeignKey(nameof(IdNotaFiscal))]
        public virtual NotaFiscal NotaFiscal { get; set; }

		[ForeignKey(nameof(IdUsuarioRecebimento))]
        public virtual AspNetUsers UsuarioRecebimento { get; set; }

    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class Quarentena
    {
        [Key]
        public long IdQuarentena { get; set; }

        public long IdLote { get; set; }

        public DateTime DataAbertura { get; set; }

        public DateTime? DataEncerramento { get; set; }

        public string Observacao { get; set; }

        public long IdQuarentenaStatus { get; set; }

        [ForeignKey(nameof(IdLote))]
        public virtual Lote Lote { get; set; }

        [ForeignKey(nameof(IdQuarentenaStatus))]
        public virtual QuarentenaStatus QuarentenaStatus { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class Quarentena
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdQuarentena { get; set; }
        [Required]
        [Index]
        public long IdLote { get; set; }
        [Required]
        public DateTime DataAbertura { get; set; }
        public DateTime? DataEncerramento { get; set; }
        [StringLength(500)]
        public string Observacao { get; set; }
        public string CodigoConfirmacao { get; set; }
        [Required]
        [Index]
        public QuarentenaStatusEnum IdQuarentenaStatus { get; set; }

        [ForeignKey(nameof(IdLote))]
        public virtual Lote Lote { get; set; }

        [ForeignKey(nameof(IdQuarentenaStatus))]
        public virtual QuarentenaStatus QuarentenaStatus { get; set; }
    }
}

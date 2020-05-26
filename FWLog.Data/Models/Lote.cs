using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class Lote
    {
        public Lote()
        {
            LoteVolumes = new HashSet<LoteVolume>();
        }

        [Key]
        [Required]
        public long IdLote { get; set; }

        [Required]
        public LoteStatusEnum IdLoteStatus { get; set; }

        [Required]
        public long IdNotaFiscal { get; set; }

        [Required]
        public DateTime DataRecebimento { get; set; }

        [Required]
        public int QuantidadeVolume { get; set; }

        [Required]
        public int QuantidadePeca { get; set; }

        [Required]
        public string IdUsuarioRecebimento { get; set; }

        [StringLength(500)]
        public string ObservacaoDivergencia { get; set; }

        public DateTime? DataInicioConferencia { get; set; }

        public DateTime? DataFinalConferencia { get; set; }

        public long? TempoTotalConferencia { get; set; }

        [ForeignKey(nameof(IdLoteStatus))]
        public virtual LoteStatus LoteStatus { get; set; }

        [ForeignKey(nameof(IdNotaFiscal))]
        public virtual NotaFiscal NotaFiscal { get; set; }

        [ForeignKey(nameof(IdUsuarioRecebimento))]
        public virtual AspNetUsers UsuarioRecebimento { get; set; }
        
        public virtual ICollection<LoteVolume> LoteVolumes { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class LoteVolume
    {
        [Key, Column(Order = 0)]
        [Required]
        public long IdLote { get; set; }

        [Key, Column(Order = 1)]
        [Required]
        public int NroVolume { get; set; }
        
        [Index]
        public long? IdEnderecoArmazenagem { get; set; }

        [Index]
        public string IdUsuarioInstalacao { get; set; }

        public DateTime? DataInstalacao { get; set; }

        [ForeignKey(nameof(IdLote))]
        public virtual Lote Lote { get; set; }
        
        [ForeignKey(nameof(IdEnderecoArmazenagem))]
        public virtual EnderecoArmazenagem EnderecoArmazenagem { get; set; }
    }
}

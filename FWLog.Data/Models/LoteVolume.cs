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

        [Index]
        public string IdUsuarioDesinstalacao { get; set; }

        public DateTime? DataInstalacao { get; set; }

        public DateTime? DataDesinstalacao { get; set; }

        [ForeignKey(nameof(IdLote))]
        public virtual Lote Lote { get; set; }
        
        [ForeignKey(nameof(IdUsuarioInstalacao))]
        public virtual AspNetUsers UsuarioInstalacao { get; set; }

        [ForeignKey(nameof(IdUsuarioDesinstalacao))]
        public virtual AspNetUsers UsuarioDesinstalacao { get; set; }

        [ForeignKey(nameof(IdEnderecoArmazenagem))]
        public virtual EnderecoArmazenagem EnderecoArmazenagem { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class Romaneio
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdRomaneio { get; set; }

        [Index]
        [Required]
        public long IdTransportadora { get; set; }

        [Index]
        [Required]
        public long IdEmpresa { get; set; }

        [Required]
        public int NroRomaneio { get; set; }

        public DateTime? DataHoraCriacao { get; set; }

        [ForeignKey(nameof(IdTransportadora))]
        public virtual Transportadora Transportadora { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }

        public virtual ICollection<RomaneioNotaFiscal> RomaneioNotaFiscal { get; set; }
    }
}

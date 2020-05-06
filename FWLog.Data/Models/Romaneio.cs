﻿using System.ComponentModel.DataAnnotations;
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
        public long NroRomaneio { get; set; }

        [ForeignKey(nameof(IdTransportadora))]
        public virtual Transportadora Transportadora { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }
    }
}

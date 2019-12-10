using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class EmpresaConfig
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdEmpresaConfig { get; set; }

        [Index]
        [Required]
        public long IdEmpresa { get; set; }

        [Index]
        [Required]
        public EmpresaTipoEnum IdEmpresaTipo { get; set; }

        [Index]
        public long? IdEmpresaMatriz { get; set; }

        [Index]
        public long? IdEmpresaGarantia { get; set; }

        [Required]
        public bool EmpresaFazGarantia { get; set; }

        [Index]
        public TipoConferenciaEnum? IdTipoConferencia { get; set; }

        [ForeignKey(nameof(IdEmpresaTipo))]
        public virtual EmpresaTipo EmpresaTipo { get; set; }

        [ForeignKey(nameof(IdEmpresaMatriz))]
        public virtual Empresa EmpresaMatriz { get; set; }

        [ForeignKey(nameof(IdEmpresaGarantia))]
        public virtual Empresa EmpresaGarantia { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey(nameof(IdTipoConferencia))]
        public virtual TipoConferencia TipoConferencia { get; set; }
    }
}

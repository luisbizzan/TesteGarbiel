using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class EmpresaConfig
    {
        [Key]
        public long IdEmpresaConfiguracao { get; set; }

        public long IdEmpresa { get; set; }

        public long IdEmpresaTipo { get; set; }

        public long? IdEmpresaMatriz { get; set; }        

        public long IdEmpresaGarantia { get; set; }

        public bool EmpresaFazGarantia { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey(nameof(IdEmpresaMatriz))]
        public virtual Empresa EmpresaMatriz { get; set; }

        [ForeignKey(nameof(IdEmpresaGarantia))]
        public virtual Empresa EmpresaGarantia { get; set; }

        [ForeignKey(nameof(IdEmpresaTipo))]
        public virtual EmpresaTipo EmpresaTipo { get; set; }
    }
}

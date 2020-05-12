using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class EmpresaConfig
    {
        [Key] 
        [Index]
        [Required]
        public long IdEmpresa { get; set; }
        [ForeignKey("IdEmpresa"), Column(Order = 0)]
        public virtual Empresa Empresa { get; set; }

        [Index]
        [Required]
        public EmpresaTipoEnum IdEmpresaTipo { get; set; }

        [Index]
        public long? IdEmpresaMatriz { get; set; }
        [ForeignKey("IdEmpresaMatriz"), Column(Order = 1)]
        public virtual Empresa EmpresaMatriz { get; set; }


        [Index]
        public long? IdEmpresaGarantia { get; set; }
        [ForeignKey("IdEmpresaGarantia"), Column(Order = 2)]
        public virtual Empresa EmpresaGarantia { get; set; }

        [Required]
        public bool EmpresaFazGarantia { get; set; }

        public string CNPJConferenciaAutomatica { get; set; }

        [Index]
        public TipoConferenciaEnum? IdTipoConferencia { get; set; }

        [Index]
        public DiasDaSemanaEnum? IdDiasDaSemana { get; set; }

        [Index]
        public long? IdTransportadora { get; set; }

        [ForeignKey(nameof(IdEmpresaTipo))]
        public virtual EmpresaTipo EmpresaTipo { get; set; }
        
        [ForeignKey(nameof(IdTipoConferencia))]
        public virtual TipoConferencia TipoConferencia { get; set; }

        [ForeignKey(nameof(IdDiasDaSemana))]
        public virtual DiasDaSemana DiasDaSemana { get; set; }

        [ForeignKey(nameof(IdTransportadora))]
        public virtual Transportadora Transportadora { get; set; }
    }
}

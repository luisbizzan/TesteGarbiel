using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class GrupoCorredorArmazenagem
    {
        [Key]
        [Index]
        [Required]
        public long IdGrupoCorredorArmazenagem { get; set; }

        [Required]
        public long IdEmpresa { get; set; }

        [Required]
        public int CorredorInicial { get; set; }

        [Required]
        public int CorredorFinal { get; set; }

        [Required]
        public long IdPontoArmazenagem { get; set; }

        [Required]
        public long IdImpressora { get; set; }

        [Required]
        public bool Ativo { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey(nameof(IdPontoArmazenagem))]
        public virtual PontoArmazenagem PontoArmazenagem { get; set; }

        [ForeignKey(nameof(IdImpressora))]
        public virtual Printer Impressora { get; set; }
    }
}

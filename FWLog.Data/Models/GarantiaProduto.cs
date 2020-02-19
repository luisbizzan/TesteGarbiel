using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class GarantiaProduto
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdGarantiaProduto { get; set; }

        [Index]
        [Required]
        public long IdGarantia { get; set; }

        [Index]
        [Required]
        public long IdNotaFiscalItem { get; set; }

        [Required]
        [Index]
        public long IdProduto { get; set; }

        [Index]
        [Required]
        public GarantiaConferenciaTipoEnum IdGarantiaConferenciaTipo { get; set; }

        [Index]
        public long IdMotivoLaudo { get; set; }

        [Index]
        [Required]
        [StringLength(128)]
        public string IdUsuarioConferencia { get; set; }

        [Required]
        public int Quantidade { get; set; }

        #region Foreign Key

        [ForeignKey(nameof(IdGarantia))]
        public virtual Garantia Garantia { get; set; }

        [ForeignKey(nameof(IdNotaFiscalItem))]
        public virtual NotaFiscalItem NotaFiscalItem { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public virtual Produto Produto { get; set; }

        [ForeignKey(nameof(IdGarantiaConferenciaTipo))]
        public virtual GarantiaConferenciaTipo GarantiaConferenciaTipo { get; set; }

        [ForeignKey(nameof(IdMotivoLaudo))]
        public virtual MotivoLaudo MotivoLaudo { get; set; }

        [ForeignKey(nameof(IdUsuarioConferencia))]
        public virtual AspNetUsers UsuarioConferencia { get; set; }

        #endregion
    }
}

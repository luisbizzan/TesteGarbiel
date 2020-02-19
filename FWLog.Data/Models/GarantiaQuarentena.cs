using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class GarantiaQuarentena
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdGarantiaQuarentena { get; set; }

        [Index]
        [Required]
        public long IdGarantia { get; set; }

        [Index]
        [Required]
        public GarantiaQuarentenaStatusEnum IdGarantiaQuarentenaStatus { get; set; }
        
        public DateTime DataCriacao { get; set; }

        public DateTime DataEncerramento { get; set; }
      
        [StringLength(500)]
        public string Observacao { get; set; }

        [StringLength(20)]
        public string CodigoConfirmacao { get; set; }

        #region Foreign Key

        [ForeignKey(nameof(IdGarantia))]
        public virtual Garantia Garantia { get; set; }

        [ForeignKey(nameof(IdGarantiaQuarentenaStatus))]
        public virtual GarantiaQuarentenaStatus GarantiaQuarentenaStatus { get; set; }

        #endregion
    }
}

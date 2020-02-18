using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class GarantiaQuarentenaHistorico
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdGarantiaQuarentenaHistorico { get; set; }

        [Index]
        [Required]
        public long IdGarantiaQuarentena { get; set; }

        [Index]
        [Required]
        [StringLength(128)]
        public string IdUsuario { get; set; }
       
        public DateTime Data { get; set; }

        [StringLength(500)]
        public string Descricao { get; set; }

        #region Foreign Key

        [ForeignKey(nameof(IdGarantiaQuarentena))]
        public virtual GarantiaQuarentena GarantiaQuarentena { get; set; }

        [ForeignKey(nameof(IdUsuario))]
        public virtual AspNetUsers Usuario { get; set; }

        #endregion
    }
}

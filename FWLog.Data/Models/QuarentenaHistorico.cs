using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class QuarentenaHistorico
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdQuarentenaHistorico { get; set; }
        [Required]
        [Index]
        public long IdQuarentena { get; set; }
        [Index]
        [StringLength(128)]
        public string IdUsuario { get; set; }
        [Required]
        public DateTime Data { get; set; }
        [Required]
        [StringLength(500)]
        public string Descricao { get; set; }

        [ForeignKey(nameof(IdQuarentena))]
        public virtual Quarentena Quarentena { get; set; }
        [ForeignKey(nameof(IdUsuario))]
        public virtual AspNetUsers Usuario { get; set; }
    }
}

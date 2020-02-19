using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class Garantia
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdGarantia { get; set; }

        [Index]
        [Required]
        public long IdNotaFiscal { get; set; }

        [Index]
        [Required]
        public GarantiaStatusEnum IdGarantiaStatus { get; set; }

        [Index]
        [Required]
        public long IdRepresentante { get; set; }

        [Index]
        [Required]
        [StringLength(128)]
        public string IdUsuarioRecebimento { get; set; }

        [Required]
        public DateTime DataRecebimento { get; set; }
   
        [StringLength(500)]
        public string Observacao { get; set; }

        [StringLength(100)]
        [Required]
        public string InformacaoTransporte { get; set; }

        public DateTime DataIncioConferencia { get; set; }

        public DateTime DataFimConferencia { get; set; }

        #region Foreign Key
        
        [ForeignKey(nameof(IdNotaFiscal))]
        public virtual NotaFiscal NotaFiscal { get; set; }

        [ForeignKey(nameof(IdGarantiaStatus))]
        public virtual GarantiaStatus GarantiaStatus { get; set; }

        [ForeignKey(nameof(IdRepresentante))]
        public virtual Representante Representante { get; set; }

        [ForeignKey(nameof(IdUsuarioRecebimento))]
        public virtual AspNetUsers UsuarioRecebimento { get; set; }

        #endregion
    }
}

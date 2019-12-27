using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class LoteDivergencia
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdLoteDivergencia { get; set; }
        [Index]
        [Required]
        public long IdLote { get; set; }
        [Index]
        [Required]
        public long IdNotaFiscal { get; set; }
        [Index]
        [Required]
        public long IdProduto { get; set; }
        [Required]
        public int QuantidadeConferencia { get; set; }
        public int? QuantidadeConferenciaMais { get; set; }
        public int? QuantidadeConferenciaMenos { get; set; }
        public int? QuantidadeDivergenciaMais { get; set; }
        public int? QuantidadeDivergenciaMenos { get; set; }
        [Index]
        public LoteDivergenciaStatusEnum IdLoteDivergenciaStatus { get; set; }
        [Index]
        public string IdUsuarioDivergencia { get; set; }
        public DateTime? DataTratamentoDivergencia { get; set; }

        [ForeignKey(nameof(IdLote))]
        public virtual Lote Lote { get; set; }
        [ForeignKey(nameof(IdNotaFiscal))]
        public virtual NotaFiscal NotaFiscal { get; set; }
        [ForeignKey(nameof(IdProduto))]
        public virtual Produto Produto { get; set; }
        [ForeignKey(nameof(IdLoteDivergenciaStatus))]
        public virtual LoteDivergenciaStatus LoteDivergenciaStatus { get; set; }
        [ForeignKey(nameof(IdUsuarioDivergencia))]
        public virtual AspNetUsers UsuarioDivergencia { get; set; }
    }
}

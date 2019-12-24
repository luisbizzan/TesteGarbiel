using FWLog.Data.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    [Log(DisplayName = "Lote")]
    public class Lote
    {
        [Key]
        [Required]
        [Log(DisplayName = "Código do Lote")]
        public long IdLote { get; set; }

        [Required]
        [Log(DisplayName = "Status do Lote")]
        public LoteStatusEnum IdLoteStatus { get; set; }

        [Required]
        [Log(DisplayName = "Código da Nota Fiscal")]
        public long IdNotaFiscal { get; set; }

        [Required]
        [Log(DisplayName = "Data de Recebimento")]
        public DateTime DataRecebimento { get; set; }

        [Required]
        [Log(DisplayName = "Quantidade de Volumes")]
        public int QuantidadeVolume { get; set; }

        [Required]
        [Log(DisplayName = "Quantidade de Peças")]
        public int QuantidadePeca { get; set; }

        [Required]
        [Log(DisplayName = "Código do Usuário Recebimento")]
        public string IdUsuarioRecebimento { get; set; }

        //[Required]
        //[Log(DisplayName = "Ganho de Estoque movimentado")]
        //public bool GanhoEstoqueEfetuado { get; set; }

        [ForeignKey(nameof(IdLoteStatus))]
        public virtual LoteStatus LoteStatus { get; set; }

        [ForeignKey(nameof(IdNotaFiscal))]
        public virtual NotaFiscal NotaFiscal { get; set; }

		[ForeignKey(nameof(IdUsuarioRecebimento))]
        public virtual AspNetUsers UsuarioRecebimento { get; set; }

    }
}

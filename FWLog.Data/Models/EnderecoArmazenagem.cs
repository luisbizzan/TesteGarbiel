using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class EnderecoArmazenagem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdEnderecoArmazenagem { get; set; }
        [Index]
        [Required]
        public long IdEmpresa { get; set; }
        [Index]
        [Required]
        public long IdNivelArmazenagem { get; set; }
        [Index]
        [Required]
        public long IdPontoArmazenagem { get; set; }
        [Index]
        [Required]
        [StringLength(10)]
        public string Codigo { get; set; }
        [Required]
        public int Corredor { get; set; }
        [Required]
        [StringLength(1)]
        public string Horizontal { get; set; }
        [Required]
        public int Vertical { get; set; }
        [Required]
        public int Divisao { get; set; }
        [Required]
        public bool IsFifo { get; set; }
        public decimal? LimitePeso { get; set; }
        [Required]
        public bool IsPontoSeparacao { get; set; }
        public int? EstoqueMinimo { get; set; }
        public int? EstoqueMaximo { get; set; }
        [Required]
        public bool Ativo { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }
        [ForeignKey(nameof(IdNivelArmazenagem))]
        public virtual NivelArmazenagem NivelArmazenagem { get; set; }
        [ForeignKey(nameof(IdPontoArmazenagem))]
        public virtual PontoArmazenagem PontoArmazenagem { get; set; }
    }
}

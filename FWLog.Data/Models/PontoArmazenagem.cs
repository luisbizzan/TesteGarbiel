using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class PontoArmazenagem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int IdPontoArmazenagem { get; set; }
        [Required]
        [Index]
        public int IdEmpresa { get; set; }
        [Required]
        [Index]
        public int IdNivelArmazenagem { get; set; }
        [Required]
        [Index]
        public int IdTipoArmazenagem { get; set; }
        [Required]
        [Index]
        public int IdTipoMovimentacao { get; set; }
        [Required]
        [StringLength(200)]
        public string Descricao { get; set; }
        public decimal? LimitePesoVertical { get; set; }
        [Required]
        public bool Ativo { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }
        [ForeignKey(nameof(IdNivelArmazenagem))]
        public virtual NivelArmazenagem NivelArmazenagem { get; set; }
        [ForeignKey(nameof(IdTipoArmazenagem))]
        public virtual TipoArmazenagem TipoArmazenagem { get; set; }
        [ForeignKey(nameof(IdTipoMovimentacao))]
        public virtual TipoMovimentacao TipoMovimentacao { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum TipoMovimentacaoEnum
    {
        Desconhecido = 0,
        Simples = 1,
        Conferencia = 2
    }

    public class TipoMovimentacao
    {
        [Key]
        [Index(IsUnique = true)]
		[Required]
        public TipoMovimentacaoEnum IdTipoMovimentacao { get; set; }
        [Index(IsUnique = true)]
        [Required]
        [StringLength(50)]
        public string Descricao { get; set; }
    }
}

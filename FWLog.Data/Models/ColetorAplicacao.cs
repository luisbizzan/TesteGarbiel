using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum ColetorAplicacaoEnum
    {
        Armazenagem = 1,
        Separacao = 2,
        Expedicao = 3,
        Recebimento = 4
    }

    public class ColetorAplicacao
    {
        [Key]
        [Index(IsUnique = true)]
        [Required]
        public ColetorAplicacaoEnum IdColetorAplicacao { get; set; }
        [StringLength(50)]
        [Index(IsUnique = true)]
        [Required]
        public string Descricao { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum AtividadeEstoqueTipoEnum
    {
        Desconhecido = 0,
        ConferenciaEndereco = 1,
        ConferenciaProdutoForaLinha = 2,
        AbastecerPicking = 3,
    }

    public class AtividadeEstoqueTipo
    {
        [Key]
        [Index(IsUnique = true)]
        [Required]
        public AtividadeEstoqueTipoEnum IdAtividadeEstoqueTipo { get; set; }

        [StringLength(50)]
        [Index(IsUnique = true)]
        [Required]
        public string Descricao { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum ColetorHistoricoTipoEnum
    {
        Login = 1,
        Logout = 2,
        InstalarProduto = 3,
        RetirarProduto = 4,
        AjustarQuantidade = 5,
        ConferirEndereco = 6,
        ImprimirEtiqueta = 7,
    }

    public class ColetorHistoricoTipo
    {
        [Key]
        [Index(IsUnique = true)]
        [Required]
        public ColetorHistoricoTipoEnum IdColetorHistoricoTipo { get; set; }
        [StringLength(50)]
        [Index(IsUnique = true)]
        [Required]
        public string Descricao { get; set; }
    }
}

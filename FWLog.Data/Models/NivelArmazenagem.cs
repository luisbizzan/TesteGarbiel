using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class NivelArmazenagem
    {
        private const string uniqueIndex = "NivelArmazenagem_UN";

        [Key]
        public long IdNivelArmazenagem { get; set; }

        [Required]
        [Index(uniqueIndex, Order = 1, IsUnique = true)]
        public long IdEmpresa { get; set; }

        [Required]
        [StringLength(1000)]
        [Index(uniqueIndex, Order = 2, IsUnique = true)]
        public string Descricao { get; set; }

        [Required]
        public bool Ativo { get; set; }

        #region ForeignKey

        [ForeignKey(nameof(IdEmpresa))]
        public Empresa Empresa { get; set; }

        #endregion
    }
}

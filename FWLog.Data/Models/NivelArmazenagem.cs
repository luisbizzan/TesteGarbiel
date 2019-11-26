using FWLog.Data.EnumsAndConsts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class NivelArmazenagem
    {
        [Key]
        public long IdNivelArmazenagem { get; set; }

        [Required]
        public long IdEmpresa { get; set; }

        [Required]
        [StringLength(1000)]
        public string Descricao { get; set; }

        [Required]
        public NaoSimEnum Ativo { get; set; }

        #region ForeignKey

        [ForeignKey(nameof(IdEmpresa))]
        public Empresa Empresa { get; set; }

        #endregion
    }
}

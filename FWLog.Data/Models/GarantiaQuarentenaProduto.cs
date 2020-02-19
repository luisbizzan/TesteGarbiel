using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class GarantiaQuarentenaProduto
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdGarantiaQuarentenaProduto { get; set; }

        [Index]
        [Required]
        public long IdGarantia { get; set; }

        [Index]
        [Required]
        public long IdProduto { get; set; }

        [Required]
        public int Quantidade { get; set; }

        #region Foreign Key

        [ForeignKey(nameof(IdGarantia))]
        public virtual Garantia Garantia { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public virtual Produto Produto { get; set; }

        #endregion
    }
}

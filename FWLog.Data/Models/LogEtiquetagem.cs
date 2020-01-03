using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class LogEtiquetagem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdLogEtiquetagem { get; set; }

        [Required]
        public long IdEmpresa { get; set; }

        public long? IdProduto { get; set; }

        [Required]
        public TipoEtiquetagemEnum IdTipoEtiquetagem { get; set; }

        [Required]
        public int Quantidade { get; set; }

        [Required]
        public DateTime DataHora { get; set; }

        [Required]
        public string IdUsuario { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public virtual Produto Produto { get; set; }

        [ForeignKey(nameof(IdTipoEtiquetagem))]
        public virtual TipoEtiquetagem TipoEtiquetagem { get; set; }

        [ForeignKey(nameof(IdUsuario))]
        public virtual AspNetUsers UsuarioEtiquetagem { get; set; }
    }
}

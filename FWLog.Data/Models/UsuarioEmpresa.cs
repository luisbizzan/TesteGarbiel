using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{   
    public class UsuarioEmpresa
    {
        [Key, Column(Order = 0)]
        [Required()]
        public string UserId { get; set; }
        [Key, Column(Order = 1)]
        [Required()]
        public long IdEmpresa { get; set; }
        [Required]
        [Index]
        public long IdPerfilUsuario { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }
        [ForeignKey(nameof(IdPerfilUsuario))]
        public virtual PerfilUsuario PerfilUsuario { get; set; }
    }
}


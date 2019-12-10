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

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }

        public UsuarioEmpresa(string userId, long idEmpresa)
        {
            UserId = userId;
            IdEmpresa = idEmpresa;
        }

        public UsuarioEmpresa()
        {

        }
    }
}


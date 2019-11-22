using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ResGen = Resources.GeneralStrings;

namespace FWLog.Data
{
    [Table("UserCompany")]
    public class UsuarioEmpresa
    {
        [Key, Column(Order = 0)]
        [Required()]
        public string UserId { get; set; }

        [Key, Column(Order = 1)]
        [Required()]
        public long CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public Empresa Empresa { get; set; }

        public UsuarioEmpresa(string userId, long idEmpresa)
        {
            UserId = userId;
            CompanyId = idEmpresa;
        }

        public UsuarioEmpresa()
        {

        }
    }
}


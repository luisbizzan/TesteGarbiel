using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ResEnt = Resources.EntityStrings;
using ResGen = Resources.GeneralStrings;

namespace FWLog.Data.Models
{
    public class PerfilUsuario
    {
        public PerfilUsuario()
        {
            UsuarioEmpresas = new HashSet<UsuarioEmpresa>();
        }

        [Key]
        [Display(Name = nameof(ResEnt.PerfilUsuarioId), ResourceType = typeof(ResEnt))]
        public long PerfilUsuarioId { get; set; }
        [Display(Name = nameof(ResEnt.UserName), ResourceType = typeof(ResEnt))]
        public string UsuarioId { get; set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResGen))]
        [Display(Name = nameof(ResEnt.EmpresaPrincipal), ResourceType = typeof(ResEnt))]
        public long? EmpresaId { get; set; }
        [StringLength(200, ErrorMessageResourceName = "InvalidMaxLenght", ErrorMessageResourceType = typeof(ResGen))]
        [Display(Name = nameof(ResEnt.Departamento), ResourceType = typeof(ResEnt))]
        public string Departamento { get; set; }
        [StringLength(200, ErrorMessageResourceName = "InvalidMaxLenght", ErrorMessageResourceType = typeof(ResGen))]
        [Display(Name = nameof(ResEnt.Cargo), ResourceType = typeof(ResEnt))]
        public string Cargo { get; set; }
        [Display(Name = nameof(ResEnt.DataNascimento), ResourceType = typeof(ResEnt))]
        public DateTime? DataNascimento { get; set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResGen))]
        [Display(Name = nameof(ResEnt.Name), ResourceType = typeof(ResEnt))]
        [StringLength(200, ErrorMessageResourceName = "InvalidMaxLenght", ErrorMessageResourceType = typeof(ResGen))]
        public string Nome { get; set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResGen))]
        [Display(Name = nameof(ResEnt.Ativo), ResourceType = typeof(ResEnt))]
        public bool Ativo { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public virtual AspNetUsers Usuario { get; set; }
        [ForeignKey(nameof(EmpresaId))]
        public virtual Empresa EmpresaPrincipal { get; set; }

        public virtual ICollection<UsuarioEmpresa> UsuarioEmpresas { get; set; }
    }
}


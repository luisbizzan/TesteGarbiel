using FWLog.Data.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ResEnt = Resources.EntityStrings;
using ResGen = Resources.GeneralStrings;

namespace FWLog.Data
{
    public class PerfilUsuario
    {
        [Key]
        [Display(Name = nameof(ResEnt.PerfilUsuarioId), ResourceType = typeof(ResEnt))]
        [Log(DisplayName = nameof(ResEnt.PerfilUsuarioId), ResourceType = typeof(ResEnt))]
        public long PerfilUsuarioId { get; set; }
                
        [Display(Name = nameof(ResEnt.UserName), ResourceType = typeof(ResEnt))]
        [Log(DisplayName = nameof(ResEnt.UserId), ResourceType = typeof(ResEnt))]
        public string UsuarioId { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResGen))]        
        [Display(Name = nameof(ResEnt.EmpresaPrincipal), ResourceType = typeof(ResEnt))]
        [Log(DisplayName = nameof(ResEnt.EmpresaPrincipal), ResourceType = typeof(ResEnt))]
        public long EmpresaId { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResGen))]
        [StringLength(200, ErrorMessageResourceName = "InvalidMaxLenght", ErrorMessageResourceType = typeof(ResGen))]
        [Display(Name = nameof(ResEnt.Departamento), ResourceType = typeof(ResEnt))]
        [Log(DisplayName = nameof(ResEnt.Departamento), ResourceType = typeof(ResEnt))]
        public string Departamento { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResGen))]
        [StringLength(200, ErrorMessageResourceName = "InvalidMaxLenght", ErrorMessageResourceType = typeof(ResGen))]
        [Display(Name = nameof(ResEnt.Cargo), ResourceType = typeof(ResEnt))]
        [Log(DisplayName = nameof(ResEnt.Cargo), ResourceType = typeof(ResEnt))]
        public string Cargo { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResGen))]
        [Display(Name = nameof(ResEnt.DataNascimento), ResourceType = typeof(ResEnt))]
        [Log(DisplayName = nameof(ResEnt.DataNascimento), ResourceType = typeof(ResEnt))]
        public DateTime DataNascimento { get; set; }
        
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResGen))]
        [Display(Name = nameof(ResEnt.Name), ResourceType = typeof(ResEnt))]
        [Log(DisplayName = nameof(ResEnt.Name), ResourceType = typeof(ResEnt))]
        [StringLength(200, ErrorMessageResourceName = "InvalidMaxLenght", ErrorMessageResourceType = typeof(ResGen))]
        public string Nome { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual AspNetUsers Usuario { get; set; }

        [ForeignKey("EmpresaId")]
        public virtual Company Company { get; set; }
    }
}


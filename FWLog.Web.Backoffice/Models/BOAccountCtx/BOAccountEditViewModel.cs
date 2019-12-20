using DartDigital.Library.Web.ModelValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Res = Resources.BOAccountStrings;

namespace FWLog.Web.Backoffice.Models.BOAccountCtx
{
    public class BOAccountEditViewModel
    {
        [Required]
        public long PerfilUsuarioId { get; set; }
        [Display(Name = nameof(Res.UserNameLabel), ResourceType = typeof(Res))]
        public string UserName { get; set; }
        [Required]
        [MvcEmailValidation]
        [Display(Name = nameof(Res.EmailLabel), ResourceType = typeof(Res))]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Nome")]
        [StringLength(200)]
        public string Nome { get; set; }
        [Required]
        [Display(Name = "Ativo?")]
        public bool Ativo { get; set; }
        [StringLength(200)]
        [Display(Name = "Departamento")]
        public string Departamento { get; set; }
        [StringLength(200)]
        [Display(Name = "Cargo")]
        public string Cargo { get; set; }
        [Display(Name = "Data Nascimento")]
        public DateTime? DataNascimento { get; set; }

        public List<EmpresaGrupoViewModel> EmpresasGrupos { get; set; } = new List<EmpresaGrupoViewModel>();
    }
}
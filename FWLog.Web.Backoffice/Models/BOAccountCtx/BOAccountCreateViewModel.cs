using DartDigital.Library.Web.ModelValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Res = Resources.BOAccountStrings;

namespace FWLog.Web.Backoffice.Models.BOAccountCtx
{
    public class BOAccountCreateViewModel
    {
        [Required]
        [Display(Name = "Código do Usuário")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "O código do usuário deve ter exatamente 5 números.")]
        public string UserName { get; set; }
        [Required]
        [MvcEmailValidation]
        [Display(Name = nameof(Res.EmailLabel), ResourceType = typeof(Res))]
        public string Email { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = nameof(Res.PasswordLabel), ResourceType = typeof(Res))]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = nameof(Res.ConfirmPasswordLabel), ResourceType = typeof(Res))]
        [Compare("Password", ErrorMessageResourceName = nameof(Res.ConfirmPasswordCompareMessage), ErrorMessageResourceType = typeof(Res))]
        public string ConfirmPassword { get; set; }
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

    public class EmpresaGrupoViewModel
    {
        [Required]
        public long IdEmpresa { get; set; }
        public string Nome { get; set; }
        public bool IsEmpresaPrincipal { get; set; }

        [Display(Name = "Perfil Impressora Padrão")]
        public long? IdPerfilImpressoraPadrao { get; set; }

        public List<GroupItemViewModel> Grupos { get; set; } = new List<GroupItemViewModel>();

        public System.Web.Mvc.SelectList PerfilImpressora { get; set; }
    }
}
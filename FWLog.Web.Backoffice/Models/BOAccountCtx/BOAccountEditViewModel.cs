using DartDigital.Library.Web.ModelValidation;
using FWLog.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Res = Resources.BOAccountStrings;

namespace FWLog.Web.Backoffice.Models.BOAccountCtx
{
    public class BOAccountEditViewModel
    {
        [Display(Name = nameof(Res.UserNameLabel), ResourceType = typeof(Res))]
        public string UserName { get; set; }

        [Required]
        [MvcEmailValidation]
        [Display(Name = nameof(Res.EmailLabel), ResourceType = typeof(Res))]
        public string Email { get; set; }

        public PerfilUsuario PerfilUsuario { get; set; }

        public List<EmpresaGrupoViewModel> EmpresasGrupos { get; set; } = new List<EmpresaGrupoViewModel>();

        public BOAccountEditViewModel()
        {
            EmpresasGrupos = new List<EmpresaGrupoViewModel>();
        }
    }
}
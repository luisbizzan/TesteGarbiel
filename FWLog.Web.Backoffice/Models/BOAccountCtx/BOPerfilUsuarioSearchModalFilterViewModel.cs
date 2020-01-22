using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BOAccountCtx
{
    public class BOPerfilUsuarioSearchModalViewModel
    {
        public BOPerfilUsuarioSearchModalItemViewModel EmptyItem { get; set; }
        public BOPerfilUsuarioSearchModalFilterViewModel Filter { get; set; }

        public BOPerfilUsuarioSearchModalViewModel()
        {
            EmptyItem = new BOPerfilUsuarioSearchModalItemViewModel();
            Filter = new BOPerfilUsuarioSearchModalFilterViewModel();
        }

        public string Origem { get; set; }
    }

    public class BOPerfilUsuarioSearchModalItemViewModel
    {
        [Display(Name = "Código")]
        public string UsuarioId { get; set; }

        [Display(Name = "Código do Usuário")]
        public string UserName { get; set; }

        [Display(Name = "Nome do Usuário")]
        public string Nome { get; set; }

        [Display(Name = "Departamento")]
        public string Departamento { get; set; }

        [Display(Name = "Cargo")]
        public string Cargo { get; set; }
    }

    public class BOPerfilUsuarioSearchModalFilterViewModel
    {
        [Display(Name = "Nome do Usuário")]
        public string Nome { get; set; }

        [Display(Name = "Código do Usuário")]
        public string UserName { get; set; }

        [Display(Name = "Departamento")]
        public string Departamento { get; set; }

        [Display(Name = "Cargo")]
        public string Cargo { get; set; }
    }
}
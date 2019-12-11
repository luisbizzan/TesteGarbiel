using System;
using System.ComponentModel.DataAnnotations;
using Res = Resources.BOAccountStrings;

namespace FWLog.Web.Backoffice.Models.BOAccountCtx
{
    public class BOAccountListViewModel
    {
        public BOAccountListItemViewModel EmptyItem { get; private set; }

        public BOAccountFilterViewModel Filter { get; set; }

        public BOAccountListViewModel()
        {
            EmptyItem = new BOAccountListItemViewModel();
            Filter = new BOAccountFilterViewModel();
        }
    }

    public class BOAccountListItemViewModel
    {
        [Display(Name = "Empresa")]
        public string NomeEmpresa { get; set; }

        [Display(Name = nameof(Res.UserNameLabel), ResourceType = typeof(Res))]
        public string UserName { get; set; }

        [Display(Name = nameof(Res.EmailLabel), ResourceType = typeof(Res))]
        public string Email { get; set; }

        [Display(Name = nameof(Res.Name), ResourceType = typeof(Res))]
        public string Nome { get; set; }

        [Display(Name = nameof(Res.CreationDateLabel), ResourceType = typeof(Res))]
        public DateTime CreationDate { get; set; }

        [Display(Name = "Status")]
        public string Ativo { get; set; }
    }

    public class BOAccountFilterViewModel
    {
        [Display(Name = nameof(Res.UserNameLabel), ResourceType = typeof(Res))]
        public string UserName { get; set; }

        [Display(Name = nameof(Res.EmailLabel), ResourceType = typeof(Res))]
        public string Email { get; set; }

        [Display(Name = nameof(Res.Name), ResourceType = typeof(Res))]
        public string Nome { get; set; }

        [Display(Name = "Status")]
        public bool? Ativo { get; set; }

        [Display(Name = "Empresa")]
        public long? IdEmpresa { get; set; }
        public string RazaoEmpresa { get; set; }
    }
}
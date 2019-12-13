using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Res = Resources.BOAccountStrings;

namespace FWLog.Web.Backoffice.Models.BOAccountCtx
{
    public class BOAccountListViewModel
    {
        public BOAccountListViewModel()
        {
            EmptyItem = new BOAccountListItemViewModel();
            Filter = new BOAccountFilterViewModel();
        }

        public BOAccountListItemViewModel EmptyItem { get; private set; }
        public BOAccountFilterViewModel Filter { get; set; }
        public SelectList Status { get; set; }
    }

    public class BOAccountListItemViewModel
    {
        [Display(Name = nameof(Res.UserNameLabel), ResourceType = typeof(Res))]
        public string UserName { get; set; }
        [Display(Name = nameof(Res.EmailLabel), ResourceType = typeof(Res))]
        public string Email { get; set; }
        [Display(Name = nameof(Res.Name), ResourceType = typeof(Res))]
        public string Nome { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
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
    }
}
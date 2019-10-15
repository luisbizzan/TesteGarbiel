using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Res = Resources.BOGroupStrings;

namespace FWLog.Web.Backoffice.Models.BOGroupCtx
{
    public class BOGroupDetailsViewModel
    {
        private List<PermissionGroupViewModel> _roleGroups = new List<PermissionGroupViewModel>();

        public string Id { get; set; }

        [Required]
        [Display(Name = nameof(Res.NameLabel), ResourceType = typeof(Res))]
        public string Name { get; set; }

        public List<PermissionGroupViewModel> RoleGroups
        {
            get { return _roleGroups; }
            set
            {
                if (RoleGroups == null)
                {
                    _roleGroups = null;
                }

                foreach (PermissionGroupViewModel group in value)
                {
                    group.Permissions = group.Permissions.OrderBy(x => x.DisplayName).ToList();
                }

                _roleGroups = value.OrderBy(x => x.DisplayName).ToList();
            }
        }

        public void SetSelectedPermissions(IEnumerable<string> permissions)
        {
            IEnumerable<PermissionItemViewModel> roleModels = RoleGroups.SelectMany(x => x.Permissions);

            foreach (PermissionItemViewModel roleModel in roleModels)
            {
                roleModel.IsSelected = permissions.Contains(roleModel.Name);
            }
        }
    }
}
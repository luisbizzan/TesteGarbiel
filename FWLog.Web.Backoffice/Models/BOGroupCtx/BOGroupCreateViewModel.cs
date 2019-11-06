using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Res = Resources.BOGroupStrings;

namespace FWLog.Web.Backoffice.Models.BOGroupCtx
{
    public class BOGroupCreateViewModel
    {
        private List<PermissionGroupViewModel> _permissionGroups = new List<PermissionGroupViewModel>();

        public string Id { get; set; }

        [Required]
        [StringLength(25)]
        [Display(Name = nameof(Res.NameLabel), ResourceType = typeof(Res))]
        public string Name { get; set; }

        public List<PermissionGroupViewModel> PermissionGroups
        {
            get { return _permissionGroups; }
            set
            {
                if (PermissionGroups == null)
                {
                    _permissionGroups = null;
                }

                foreach (PermissionGroupViewModel group in value)
                {
                    group.Permissions = group.Permissions.OrderBy(x => x.DisplayName).ToList();
                }

                _permissionGroups = value.OrderBy(x => x.DisplayName).ToList();
            }
        }

        public List<PermissionItemViewModel> GetSelectedRoles()
        {
            if (PermissionGroups == null)
                return new List<PermissionItemViewModel>();

            return PermissionGroups
                .SelectMany(x => x.Permissions)
                .Where(x => x.IsSelected)
                .ToList();
        }

        public void SetSelectedPermissions(IEnumerable<string> permissions)
        {
            IEnumerable<PermissionItemViewModel> permissionModels = PermissionGroups.SelectMany(x => x.Permissions);

            foreach (PermissionItemViewModel model in permissionModels)
            {
                model.IsSelected = permissions.Contains(model.Name);
            }
        }
    }

    public class PermissionGroupViewModel
    {
        public string DisplayName { get; set; }
        public List<PermissionItemViewModel> Permissions { get; set; }
    }

    public class PermissionItemViewModel
    {
        public bool IsSelected { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
    }
}
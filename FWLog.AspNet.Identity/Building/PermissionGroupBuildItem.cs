using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.AspNet.Identity.Building
{
    public class PermissionGroupBuildItem
    {
        private Display _display;
        private List<PermissionBuildItem> _permissions;
        private ResourceManager _resourceManager;

        public IEnumerable<PermissionBuildItem> Permissions { get => _permissions; }

        public PermissionGroupBuildItem() : this(null)
        {

        }

        public PermissionGroupBuildItem(Display display)
        {
            _display = display;
            _permissions = new List<PermissionBuildItem>();
            _resourceManager = null;
        }

        public string GetDisplayName()
        {
            return _display != null ? _display.GetDisplayName(_resourceManager) : string.Empty;
        }

        internal void SetResourceManager(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
            _permissions.ForEach(x => x.SetResourceManager(resourceManager));
        }

        protected void Register(string permissionName, Display display)
        {
            var permission = new PermissionBuildItem(permissionName, display);

            if (_resourceManager != null)
            {
                permission.SetResourceManager(_resourceManager);
            }

            _permissions.Add(permission);
        }

        protected void Register(params string[] permissionNames)
        {
            if (permissionNames == null)
            {
                return;
            }

            foreach (string name in permissionNames)
            {
                Register(name, (Display)null);
            }
        }
    }
}

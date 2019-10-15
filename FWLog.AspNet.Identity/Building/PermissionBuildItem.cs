using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.AspNet.Identity.Building
{
    public class PermissionBuildItem
    {
        private string _name;
        private Display _display;
        private ResourceManager _resourceManager;

        public string Name { get => _name; }

        public PermissionBuildItem(string name, Display display)
        {
            _name = name;
            _display = display;
            _resourceManager = null;
        }

        public string GetDisplayName()
        {
            return _display != null ? _display.GetDisplayName(_resourceManager) : string.Empty;
        }

        internal void SetResourceManager(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }
    }
}

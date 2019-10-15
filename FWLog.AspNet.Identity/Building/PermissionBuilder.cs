using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.AspNet.Identity.Building
{
    public class PermissionBuilder
    {
        private ResourceManager _resourceManager;

        public IEnumerable<PermissionGroupBuildItem> Groups { get; }

        public PermissionBuilder() : this(null)
        {

        }

        public PermissionBuilder(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
            Groups = LoadNestedGroups();
        }

        private IEnumerable<PermissionGroupBuildItem> LoadNestedGroups()
        {
            Type[] nestedTypes = this.GetType().GetNestedTypes().Where(x => x.IsSubclassOf(typeof(PermissionGroupBuildItem))).ToArray();
            var groups = new List<PermissionGroupBuildItem>();

            foreach (Type type in nestedTypes)
            {
                PermissionGroupBuildItem instance = (PermissionGroupBuildItem)Activator.CreateInstance(type);
                instance.SetResourceManager(_resourceManager);
                groups.Add(instance);
            }

            return groups;
        }
    }
}

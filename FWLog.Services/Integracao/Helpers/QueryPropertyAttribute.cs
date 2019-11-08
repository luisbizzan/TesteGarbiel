using System;
using System.Reflection;

namespace FWLog.Services.Integracao.Helpers
{
    public class QueryPropertyAttribute : Attribute
    {
        public string DisplayName { get; set; }
        public Type ResourceType { get; set; }

        public string GetDisplayName()
        {
            if (ResourceType == null)
            {
                return DisplayName;
            }

            if (String.IsNullOrEmpty(DisplayName))
            {
                return DisplayName;
            }

            return GetResourceStringFromType(ResourceType, DisplayName);
        }

        private static string GetResourceStringFromType(Type resourceType, string resourceKey)
        {
            foreach (PropertyInfo staticProperty in resourceType.GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                if (staticProperty.PropertyType == typeof(System.Resources.ResourceManager))
                {
                    System.Resources.ResourceManager resourceManager = (System.Resources.ResourceManager)staticProperty.GetValue(null, null);
                    return resourceManager.GetString(resourceKey);
                }
            }

            return resourceKey; // Fallback with the key name
        }
    }
}

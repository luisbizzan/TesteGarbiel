using System;
using System.Resources;

namespace FWLog.AspNet.Identity.Building
{
    public class Display
    {
        private string Value { get; }

        public bool IsFromResource { get; }

        private Display(string value, bool isFromResource)
        {
            Value = value;
            IsFromResource = isFromResource;
        }

        public static Display FromResource(string key)
        {
            return new Display(key, true);
        }

        public static Display FromString(string text)
        {
            return new Display(text, false);
        }

        public string GetDisplayName(ResourceManager resourceManager)
        {
            if (IsFromResource)
            {
                return resourceManager != null ? resourceManager.GetString(Value) : throw new InvalidOperationException("The parameter resourceManager needs to be set");
            }
            else
            {
                return Value;
            }
        }
    }
}

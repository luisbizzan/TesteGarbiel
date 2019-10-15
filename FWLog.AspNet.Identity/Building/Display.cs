using System;
using System.Resources;

namespace FWLog.AspNet.Identity.Building
{
    public class Display
    {
        private string _value;

        public bool IsFromResource { get; }

        private Display(string value, bool isFromResource)
        {
            _value = value;
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
                return resourceManager != null ? resourceManager.GetString(_value) : throw new InvalidOperationException("The parameter resourceManager needs to be set");
            }
            else
            {
                return _value;
            }
        }
    }
}

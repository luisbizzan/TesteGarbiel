using System.Text.RegularExpressions;

namespace FWLog.Data.ExtensionMethods
{
    public static class StringExtension
    {
        public static bool Contem(this string source, string match, RegexOptions options = RegexOptions.IgnoreCase)
        {
            return Regex.IsMatch(source, Regex.Escape(match), options);
        }
    }
}

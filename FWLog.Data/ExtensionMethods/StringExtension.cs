using System.Text.RegularExpressions;
using Res = Resources.GeneralStrings;

namespace ExtensionMethods.String
{
    public static class StringExtension
    {
        public static bool Contem(this string source, string match, RegexOptions options = RegexOptions.IgnoreCase)
        {
            return Regex.IsMatch(source, Regex.Escape(match), options);
        }

        public static string BooleanResource(this bool value)
        {
            if (value)
            {
                return Res.Yes;
            }
            else
            {
                return Res.No;
            }
        }
    }
}

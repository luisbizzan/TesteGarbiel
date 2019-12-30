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

        public static string PadBoth(this string source, int length, char paddingChar = (char)32)
        {
            int spaces = length - source.Length;
            int padLeft = spaces / 2 + source.Length;

            return source.PadLeft(padLeft, paddingChar).PadRight(length, paddingChar);
        }
    }
}

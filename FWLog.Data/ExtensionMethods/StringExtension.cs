using System.Text;
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

        public static string Normalizar(this string source)
        {
            byte[] tempBytes = Encoding.GetEncoding("ISO-8859-8").GetBytes(source);
            return Encoding.UTF8.GetString(tempBytes);
        }

        public static string CNPJ(this string source)
        {
            // TODO: checar o valor de entrada
            var pattern = @"^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$";
            var regExp = new Regex(pattern);
            return regExp.Replace(source, "$1.$2.$3/$4-$5");
        }

        public static string Telefone(this string source)
        {
            // TODO: checar o valor de entrada
            var pattern = @"^(\d{2})(\d{4,5})(\d{4})$";
            var regExp = new Regex(pattern);
            return regExp.Replace(source, "($1) $2-$3");
        }
    }
}

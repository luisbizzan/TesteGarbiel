using System;
using System.Text;
using System.Text.RegularExpressions;
using Res = Resources.GeneralStrings;

namespace ExtensionMethods.String
{
    public static class StringExtension
    {
        /// <summary>
        /// Retorna um valor indicando se uma substring especificada ocorre dentro dessa string.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="match">String a procurar</param>
        /// <param name="options">Opções de comparação. Default: IgnoreCase</param>
        /// <returns></returns>
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

        /// <summary>
        /// Retorna uma nova string que alinha os caracteres ao centro nesta instância, preenchendo-os com caractere Unicode especificado à esquerda e à direita, por um comprimento total especificado.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="length">Comprimento total</param>
        /// <param name="paddingChar">Caractere Unicode para preenchimento. Default: ' '</param>
        /// <returns></returns>
        public static string PadBoth(this string source, int length, char paddingChar = (char)32)
        {
            int spaces = length - source.Length;
            int padLeft = spaces / 2 + source.Length;

            return source.PadLeft(padLeft, paddingChar).PadRight(length, paddingChar);
        }

        /// <summary>
        /// Retorna uma nova sequência cujo valor textual é o mesmo que essa sequência, mas cuja representação binária está no formulário de normalização ISO-8859-8.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Normalizar(this string source)
        {
            byte[] tempBytes = Encoding.GetEncoding("ISO-8859-8").GetBytes(source);
            return Encoding.UTF8.GetString(tempBytes);
        }

        /// <summary>
        /// Retorna uma nova sequência aplicando a máscara de CNPJ. A sequência deve conter exatamente 14 dígitos.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string MascaraCNPJ(this string source)
        {
            string pattern = @"^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$";
            var regExp = new Regex(pattern);
            return regExp.Replace(source, "$1.$2.$3/$4-$5");
        }

        /// <summary>
        /// Retorna uma nova sequência aplicando a máscara de Telefone/Celular. A sequência deve conter exatamente 10 ou 11 dígitos.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string MascaraTelefone(this string source)
        {
            string pattern = @"^(\d{2})(\d{4,5})(\d{4})$";
            var regExp = new Regex(pattern);
            return regExp.Replace(source, "($1) $2-$3");
        }

        /// <summary>
        /// Retorna cpf ou cnpj já com a máscara.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string CnpjOrCpf(this string source)
        {
            if (source.Length > 11)
                return Convert.ToUInt64(source).ToString(@"00\.000\.000\/0000\-00");

            return Convert.ToUInt64(source).ToString(@"000\.000\.000\-00");
        }
    }
}

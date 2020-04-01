using System.Collections.Generic;
using System.Text;

namespace FWLog.Web.Api.Models
{
    public class ApiErroResposta
    {
        public ApiErroResposta()
        {
            Erros = new List<ApiErro>();
        }

        public List<ApiErro> Erros { get; set; }

        public override string ToString()
        {
            if (Erros == null || Erros.Count == 0)
            {
                return base.ToString();
            }
            else
            {
                StringBuilder retornar = new StringBuilder();

                foreach (var erro in Erros)
                {
                    retornar.Append(string.Concat(erro.Mensagem, " - "));
                }

                return retornar.ToString();
            }
        }
    }

    public class ApiErro
    {
        public string Mensagem { get; set; }
    }
}
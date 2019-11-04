using System.Collections.Generic;

namespace FWLog.Web.Api.Models.Usuario
{
    public class LoginModelResponse
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public List<Empresa> Empresas { get; set; }
    }

    public class Empresa
    {
        public int IdEmpresa { get; set; }
        public string Nome { get; set; }
    }
}
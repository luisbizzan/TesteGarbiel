using System.Collections.Generic;

namespace FWLog.Web.Api.Models.Usuario
{
    public class LoginModelResponse
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public List<EmpresaModelResponse> Empresas { get; set; }
    }

    public class EmpresaModelResponse
    {
        public long IdEmpresa { get; set; }
        public string Sigla { get; set; }
    }
}
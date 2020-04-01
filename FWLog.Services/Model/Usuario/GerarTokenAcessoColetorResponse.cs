using FWLog.Data.Models;
using System.Collections.Generic;

namespace FWLog.Services.Model.Usuario
{
    public class GerarTokenAcessoColetorResponse
    {
        public TokenResponse Token { get; set; }
        public ApplicationSession ApplicationSession { get; set; }
        public List<UsuarioEmpresa> EmpresasUsuario { get; set; }
    }
}

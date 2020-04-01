using System.Collections.Generic;

namespace FWLog.Web.Api.Models.Usuario
{
    public class BuscarPermissoesResposta
    {
        public string IdUsuario { get; set; }
        public long IdEmpresa { get; set; }
        public IList<string> Permissoes { get; set; }
    }
}
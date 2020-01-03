using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FWLog.Web.Api.Models
{
    public class EmpresaResposta
    {
        public long IdEmpresa { get; set; }
        public string RazaoSocial { get; set; }
        public string Sigla { get; set; }
        public string NomeFantasia { get; set; }
        public string CNPJ { get; set; }
        public long CodigoIntegracao { get; set; }
    }
}
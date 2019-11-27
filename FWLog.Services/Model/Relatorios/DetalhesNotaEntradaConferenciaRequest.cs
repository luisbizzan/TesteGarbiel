using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Services.Model.Relatorios
{
    public class DetalhesNotaEntradaConferenciaRequest
    {
        public long IdEmpresa { get; set; }
        public string NomeUsuario { get; set; }
        public long IdNotaFiscal { get; set; }
    }
}

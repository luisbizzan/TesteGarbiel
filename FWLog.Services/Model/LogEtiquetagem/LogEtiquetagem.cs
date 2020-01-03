using FWLog.Data.Models;
using System;

namespace FWLog.Services.Model.LogEtiquetagem
{
    public class LogEtiquetagem
    {
        public long IdLogEtiquetagem { get; set; }
    
        public long IdEmpresa { get; set; }

        public long? IdProduto { get; set; }

        public int IdTipoEtiquetagem { get; set; }

        public int Quantidade { get; set; }

        public DateTime DataHora { get; set; }

        public string IdUsuario { get; set; }
    }
}

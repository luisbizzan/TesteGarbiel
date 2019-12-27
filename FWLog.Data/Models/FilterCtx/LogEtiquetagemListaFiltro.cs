using System;

namespace FWLog.Data.Models.FilterCtx
{
    public class LogEtiquetagemListaFiltro
    {
        public DateTime? DataInicial { get; set; }
        
        public DateTime? DataFinal { get; set; }

        public TipoEtiquetagemEnum IdLogEtiquetagem { get; set; }

        public string IdUsuarioEtiquetagem { get; set; }        
    }
}

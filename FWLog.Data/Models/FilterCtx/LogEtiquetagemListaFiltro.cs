using System;

namespace FWLog.Data.Models.FilterCtx
{
    public class LogEtiquetagemListaFiltro
    {
        public long? IdProduto { get; set; }
        
        public DateTime? DataInicial { get; set; }
        
        public DateTime? DataFinal { get; set; }
        
        public int? QuantidadeInicial { get; set; }
        
        public int? QuantidadeFinal { get; set; }
        
        public string IdUsuarioEtiquetagem { get; set; }
    }
}

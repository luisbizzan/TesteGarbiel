using System;

namespace FWLog.Data.Models.FilterCtx
{
    public class HistoricoAcaoUsuarioFilter
    {
        public int? IdColetorAplicacao { get; set; }
        public int? IdHistoricoColetorTipo { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public string IdUsuario { get; set; }
        public long IdEmpresa { get; set; }
    }
}

using System.Collections.Generic;

namespace FWLog.Data.Models.FilterCtx
{
    public class RelatorioTotalizacaoAlasListaFiltro
    {
        public long IdEmpresa { get; set; }
        public int CorredorInicial { get; set; }
        public int CorredorFinal { get; set; }
        public bool ImprimirVazia { get; set; }
        public IEnumerable<long> ListaIdEnderecoArmazenagem { get; set; }
    }
}

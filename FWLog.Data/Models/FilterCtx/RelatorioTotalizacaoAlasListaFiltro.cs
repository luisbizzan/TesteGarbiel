using System.Collections.Generic;

namespace FWLog.Data.Models.FilterCtx
{
    public class RelatorioTotalizacaoAlasListaFiltro
    {
        public long IdEmpresa { get; set; }
        public int CorredorInicial { get; set; }
        public int CorredorFinal { get; set; }
        public bool ImprimirVazia { get; set; }
        public long IdPontoArmazenagem { get; set; }
        public long IdNivelArmazenagem { get; set; }
        public IEnumerable<EnderecoArmazenagem> ListaIdEnderecoArmazenagem { get; set; }
    }
}

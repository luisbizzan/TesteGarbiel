using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Data.Models.FilterCtx
{
    public class PontoArmazenagemListaFiltro
    {
        public long IdEmpresa { get; set; }
        public string Descricao { get; set; }
        public long? IdNivelArmazenagem { get; set; }
        public TipoArmazenagemEnum? IdTipoArmazenagem { get; set; }
        public TipoMovimentacaoEnum? IdTipoMovimentacao { get; set; }
        public bool? Status { get; set; }
    }
}

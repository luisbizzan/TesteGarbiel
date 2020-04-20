using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Data.Models.FilterCtx
{
    public class AtividadeEstoqueListaFiltro
    {
        public long IdEmpresa { get; set; }

        public long? IdProduto { get; set; }

        public int? IdAtividadeEstoqueTipo { get; set; }

        public int? QuantidadeInicial { get; set; }

        public int? QuantidadeFinal { get; set; }

        public DateTime? DataInicialSolicitacao { get; set; }

        public DateTime? DataFinalSolicitacao { get; set; }

        public DateTime? DataInicialExecucao { get; set; }

        public DateTime? DataFinalExecucao { get; set; }

        public string IdUsuarioExecucao { get; set; }
    }
}

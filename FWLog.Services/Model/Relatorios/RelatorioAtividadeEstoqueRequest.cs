using FWLog.Data.Models.FilterCtx;
using System;

namespace FWLog.Services.Model.Relatorios
{
    public class RelatorioAtividadeEstoqueRequest 
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

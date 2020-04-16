using System;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class AtividadeEstoqueListaTabela
    {
        public string TipoAtividade { get; set; }

        public string CodigoEndereco { get; set; }

        public string ReferenciaProduto { get; set; }

        public string DescricaoProduto { get; set; }

        public int? QuantidadeInicial { get; set; }

        public DateTime? DataSolicitacao { get; set; }

        public int? QuantidadeFinal { get; set; }

        public DateTime? DataExecucao { get; set; }

        public string UsuarioExecucao { get; set; }

        public bool Finalizado { get; set; }
    }
}

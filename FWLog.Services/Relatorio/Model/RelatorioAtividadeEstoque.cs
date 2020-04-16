using System;

namespace FWLog.Services.Relatorio.Model
{
    public class RelatorioAtividadeEstoque : IFwRelatorioDados
    {
        [ColunaRelatorio(Nome = "Empresa", Tamanho = 150)]
        public string Empresa { get; set; }

        [ColunaRelatorio(Nome = "Tipo", Tamanho = 80)]
        public string TipoAtividade { get; set; }

        [ColunaRelatorio(Nome = "Endereço", Tamanho = 80)]
        public string CodigoEndereco { get; set; }

        [ColunaRelatorio(Nome = "Produto", Tamanho = 250)]
        public string ReferenciaDescricaoProduto { get; set; }

        [ColunaRelatorio(Nome = "Qtde. Inicial", Tamanho = 80)]
        public int? QuantidadeInicial { get; set; }

        [ColunaRelatorio(Nome = "Data Solic.", Tamanho = 80)]
        public DateTime? DataSolicitacao { get; set; }

        [ColunaRelatorio(Nome = "Qtde. Final", Tamanho = 80)]
        public int? QuantidadeFinal { get; set; }

        [ColunaRelatorio(Nome = "Data Exec.", Tamanho = 80)]
        public DateTime? DataExecucao { get; set; }

        [ColunaRelatorio(Nome = "Usuário", Tamanho = 80)]
        public string UsuarioExecucao { get; set; }

        [ColunaRelatorio(Nome = "Finalizado", Tamanho = 80)]
        public bool Finalizado { get; set; }
    }
}

namespace FWLog.Services.Relatorio.Model
{
    public class RelatorioAtividadeEstoque : IFwRelatorioDados
    {
        [ColunaRelatorio(Nome = "Tipo", Tamanho = 70)]
        public string TipoAtividade { get; set; }

        [ColunaRelatorio(Nome = "Produto", Tamanho = 180)]
        public string ReferenciaDescricaoProduto { get; set; }

        [ColunaRelatorio(Nome = "Endereço", Tamanho = 60)]
        public string CodigoEndereco { get; set; }

        [ColunaRelatorio(Nome = "Qtd. Inicial", Tamanho = 70)]
        public string QuantidadeInicial { get; set; }

        [ColunaRelatorio(Nome = "Qtd. Final", Tamanho = 60)]
        public string QuantidadeFinal { get; set; }

        [ColunaRelatorio(Nome = "% Div.", Tamanho = 45)]
        public string PorcentagemDivergencia { get; set; }

        [ColunaRelatorio(Nome = "Data Solic.", Tamanho = 70)]
        public string DataSolicitacao { get; set; }

        [ColunaRelatorio(Nome = "Data Exec.", Tamanho = 70)]
        public string DataExecucao { get; set; }

        [ColunaRelatorio(Nome = "Usuário", Tamanho = 80)]
        public string UsuarioExecucao { get; set; }

        [ColunaRelatorio(Nome = "Finalizado", Tamanho = 70)]
        public string Finalizado { get; set; }
    }
}
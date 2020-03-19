namespace FWLog.Services.Model.Lote
{
    public class ProcessamentoTratativaDivergencia
    {
        public bool AtualizacaoNFCompra { get; set; }
        public bool ConfirmacaoNFCompra { get; set; }
        public bool AtualizacaoEstoque { get; set; }
        public bool CriacaoQuarentena { get; set; }
        public bool CriacaoNFDevolucao { get; set; }
        public bool ConfirmacaoNFDevolucao { get; set; }
        public bool AutorizaçãoNFDevolucaoSefaz { get; set; }

        public bool ProcessamentoErro { get; set; }
        public string ProcessamentoErroMensagem { get; set; }
    }
}

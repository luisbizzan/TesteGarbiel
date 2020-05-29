namespace FWLog.Services.Model.Lote
{
    public class ProcessamentoDevolucaoTotal
    {
        public bool AtualizacaoNFCompra { get; set; }
        public bool ConfirmacaoNFCompra { get; set; }
        public bool CriacaoQuarentena { get; set; }
        public bool CriacaoNFDevolucao { get; set; }
        public bool ConfirmacaoNFDevolucao { get; set; }
        public bool AutorizacaoNFDevolucaoSefaz { get; set; }
        public bool ProcessamentoErro { get; set; }
        public string ProcessamentoErroMensagem { get; set; }
    }
}

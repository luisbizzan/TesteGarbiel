using System;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class BORegistroRecebimentoViewModel
    {
        public long IdNotaFiscal { get; set; }
        public string NumeroSerieNotaFiscal { get; set; }        
        public string HoraRecebimento { get; set; }
        public string DataRecebimento { get; set; }
        public DateTime DataAtual { get; set; }
        public string FornecedorNome { get; set; }
        public string ChaveAcesso { get; set; }
        public string ValorTotal { get; set; }
        public string ValorFrete { get; set; }
        public long? NumeroConhecimento { get; set; }
        public int? QtdVolumes { get; set; }
        public string Peso { get; set; }
        public string TransportadoraNome { get; set; }
        public bool NotaFiscalPesquisada { get; set; }
    }
}
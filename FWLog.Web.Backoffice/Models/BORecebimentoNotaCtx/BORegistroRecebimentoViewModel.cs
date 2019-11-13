using System;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class BORegistroRecebimentoViewModel
    {
        public long IdNotaFiscal { get; set; }
        public string NumeroSerieNotaFiscal { get; set; }        
        public string HoraRecebimento { get; set; }
        public string DataRecebimento { get; set; }
        public string DataAtual { get; set; }
        public string FornecedorNome { get; set; }
        public string ChaveAcesso { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorFrete { get; set; }
        public long NumeroConhecimento { get; set; }
        public int QtdVolumes { get; set; }
        public decimal Peso { get; set; }
        public string TransportadoraNome { get; set; }
        public bool ExibeCamposFrete { get; set; }
    }
}
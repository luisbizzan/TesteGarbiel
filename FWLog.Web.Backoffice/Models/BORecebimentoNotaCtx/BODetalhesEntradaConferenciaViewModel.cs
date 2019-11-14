using System.Collections.Generic;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class BODetalhesEntradaConferenciaViewModel
    {
        public bool IsNotaRecebida { get; set; }
        public bool IsNotaConferida { get; set; }
        public bool IsNotaDivergente { get; set; }
        public string DANFE { get; set; }
        public string NumeroLote { get; set; }
        public string NumeroNotaFiscal { get; set; }
        public string DataChegada { get; set; }
        public string StatusNotaFiscal { get; set; }
        public string Fornecedor { get; set; }
        public string Quantidade { get; set; }
        public string DiasAtraso { get; set; }
        public string DataCompra { get; set; }
        public string Volumes { get; set; }
        public string PrazoRecebimento { get; set; }
        public string FornecedorCNPJ { get; set; }
        public string UsuarioRecebimento { get; set; }
        public string ValorTotal { get; set; }
        public string ValorFrete { get; set; }
        public string NumeroConhecimento { get; set; }
        public string PesoConhecimento { get; set; }
        public string TransportadoraNome { get; set; }
        public string ConferenciaTipo { get; set; }
        public string UsuarioConferencia { get; set; }
        public string DataInicioConferencia { get; set; }
        public string DataFimConferencia { get; set; }
        public string TempoTotalConferencia { get; set; }

        public List<BODetalhesEntradaConferenciaItem> Items { get; set; }
    }

    public class BODetalhesEntradaConferenciaItem
    {
        public string Referencia { get; set; }
        public string Quantidade { get; set; }
        public string DataInicioConferencia { get; set; }
        public string DataFimConferencia { get; set; }
        public string UsuarioConferencia { get; set; }
        public string TempoTotalConferencia { get; set; }
    }
}
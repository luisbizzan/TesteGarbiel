namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class BODetalhesEtiquetaConferenciaViewModel
    {
        public string NomeFornecedor { get; set; }
        public string NumeroNotaFiscal { get; set; }
        public string DataHoraRecebimento { get; set; }
        public string QuantidadeVolumes { get; set; }
        public string Referencia { get; set; }
        public string Embalagem { get; set; }
        public string Unidade { get; set; }
        public string Multiplo { get; set; }
        public int QuantidadePorVolumes { get; set; }
        public int NumerosDeCaixas { get; set; }
        public int TotalItens { get; set; }
        public decimal MediaVendaMes { get; set; }
        public int EStoque { get; set; }
        public string Localizacao { get; set; }
        public int QuantidadeNaoConferida { get; set; }
        public int QuantidadeConferida { get; set; }
        public int ReservaVendas { get; set; }
        public int Apoio { get; set; }

    }
}
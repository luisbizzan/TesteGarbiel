using FWLog.Services.Integracao.Helpers;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFCAB")]
    public class NotaFiscalIntegracao
    {
        [TabelaIntegracao(DisplayName = "TGFCAB.NUNOTA")]
        public string NUNOTA { get; set; } //CodigoIntegracao

        [TabelaIntegracao(DisplayName = "TGFCAB.NUMNOTA")]
        public string NUMNOTA { get; set; } //Numero

        [TabelaIntegracao(DisplayName = "TGFCAB.SERIENOTA")]
        public string SERIENOTA { get; set; } //Serie

        [TabelaIntegracao(DisplayName = "TGFCAB.CODEMP")]
        public string CODEMP { get; set; } //CompanyId

        [TabelaIntegracao(DisplayName = "TGFCAB.DANFE")]
        public string DANFE { get; set; } //DANFE

        [TabelaIntegracao(DisplayName = "TGFCAB.CHAVENFE")]
        public string CHAVENFE { get; set; } //Chave

        [TabelaIntegracao(DisplayName = "TGFCAB.VLRNOTA")]
        public string VLRNOTA { get; set; } //ValorTotal

        [TabelaIntegracao(DisplayName = "TGFCAB.CIF_FOB")]
        public string CIF_FOB { get; set; } //IdFreteTipo

        [TabelaIntegracao(DisplayName = "TGFCAB.CODPARC")]
        public string CODPARC { get; set; } //IdFornecedor

        [TabelaIntegracao(DisplayName = "TGFCAB.STATUSNOTA")]
        public string STATUSNOTA { get; set; } //StatusIntegracao

        [TabelaIntegracao(DisplayName = "TGFCAB.CODPARCTRANSP")]
        public string CODPARCTRANSP { get; set; } //IdTransportadora       

        [TabelaIntegracao(DisplayName = "TGFCAB.VLRFRETE")]
        public string VLRFRETE { get; set; } //ValorFrete

        [TabelaIntegracao(DisplayName = "TGFCAB.NUMCF")]
        public string NUMCF { get; set; } //NumeroConhecimento

        [TabelaIntegracao(DisplayName = "TGFCAB.PESOBRUTO")]
        public string PESOBRUTO { get; set; } //PesoBruto

        [TabelaIntegracao(DisplayName = "TGFCAB.VOLUME")]
        public string VOLUME { get; set; } //Especie

        [TabelaIntegracao(DisplayName = "TGFCAB.QTDVOL")] //TODO ALTERAR NOME
        public string QTDVOL { get; set; } //Quantidade

        [TabelaIntegracao(DisplayName = "TGFCAB.DHEMISSEPEC")]
        public string DHEMISSEPEC { get; set; } //DataEmissao

        [TabelaIntegracao(DisplayName = "TGFITE.CODPROD")]
        public string CODPROD { get; set; }

        [TabelaIntegracao(DisplayName = "TGFITE.CODVOL")]
        public string CODVOL { get; set; }

        [TabelaIntegracao(DisplayName = "TGFITE.QTDNEG")]
        public string QTDNEG { get; set; }

        [TabelaIntegracao(DisplayName = "TGFITE.VLRUNIT")]
        public string VLRUNIT { get; set; }

        [TabelaIntegracao(DisplayName = "TGFITE.VLRTOT")]
        public string VLRTOT { get; set; }
    }

}



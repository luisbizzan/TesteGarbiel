using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFCAB")]
    public class NotaFiscalIntegracao
    {
        [TabelaIntegracao(DisplayName = "TGFCAB.NUNOTA")]
        [Required]
        public string NUNOTA { get; set; } //CodigoIntegracao

        [TabelaIntegracao(DisplayName = "TGFCAB.NUMNOTA")]
        [Required]
        public string NUMNOTA { get; set; } //Numero

        [TabelaIntegracao(DisplayName = "TGFCAB.SERIENOTA")]
        public string SERIENOTA { get; set; } //Serie

        [TabelaIntegracao(DisplayName = "TGFCAB.CODEMP")]
        [Required]
        public string CODEMP { get; set; } //CompanyId

        [TabelaIntegracao(DisplayName = "TGFCAB.CHAVENFE")]
        public string CHAVENFE { get; set; } //Chave

        [TabelaIntegracao(DisplayName = "TGFCAB.VLRNOTA")]
        [Required]
        public string VLRNOTA { get; set; } //ValorTotal

        [TabelaIntegracao(DisplayName = "TGFCAB.CIF_FOB")]
        [Required]
        public string CIF_FOB { get; set; } //IdFreteTipo

        [TabelaIntegracao(DisplayName = "TGFCAB.CODPARC")]
        [Required]
        public string CODPARC { get; set; } //IdFornecedor

        [TabelaIntegracao(DisplayName = "TGFCAB.STATUSNOTA")]
        [Required]
        public string STATUSNOTA { get; set; } //StatusIntegracao

        [TabelaIntegracao(DisplayName = "TGFCAB.CODPARCTRANSP")]
        [Required]
        public string CODPARCTRANSP { get; set; } //IdTransportadora       

        [TabelaIntegracao(DisplayName = "TGFCAB.VLRFRETE")]
        [Required]
        public string VLRFRETE { get; set; } //ValorFrete

        [TabelaIntegracao(DisplayName = "TGFCAB.NUMCF")]
        public string NUMCF { get; set; } //NumeroConhecimento

        [TabelaIntegracao(DisplayName = "TGFCAB.PESOBRUTO")]
        public string PESOBRUTO { get; set; } //PesoBruto

        [TabelaIntegracao(DisplayName = "TGFCAB.VOLUME")]
        public string VOLUME { get; set; } //Especie

        [TabelaIntegracao(DisplayName = "TGFCAB.QTDVOL")] //TODO ALTERAR NOME
        [Required]
        public string QTDVOL { get; set; } //Quantidade

        [TabelaIntegracao(DisplayName = "TGFCAB.DTNEG")]
        public string DTNEG { get; set; } //DataEmissao

        [TabelaIntegracao(DisplayName = "TGFCAB.AD_NUMFICTSAV")]
        public string NumeroNFSAV { get; set; } 

        [TabelaIntegracao(DisplayName = "TGFCAB.CODVEND")]
        public string CodigoIntegracaoVendedor { get; set; }

        [TabelaIntegracao(DisplayName = "TGFITE.CODPROD")]
        [Required]
        public string CODPROD { get; set; }

        [TabelaIntegracao(DisplayName = "TGFITE.CODVOL")]
        [Required]
        public string CODVOL { get; set; }

        [TabelaIntegracao(DisplayName = "TGFITE.QTDNEG")]
        [Required]
        public string QTDNEG { get; set; }

        [TabelaIntegracao(DisplayName = "TGFITE.VLRUNIT")]
        [Required]
        public string VLRUNIT { get; set; }

        [TabelaIntegracao(DisplayName = "TGFITE.VLRTOT")]
        [Required]
        public string VLRTOT { get; set; }
    }

}



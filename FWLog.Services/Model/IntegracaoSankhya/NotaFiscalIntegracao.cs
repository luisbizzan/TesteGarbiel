using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFCAB")]
    public class NotaFiscalIntegracao
    {
        [TabelaIntegracao(DisplayName = "TGFCAB.NUNOTA")]
        [Required]
        public string CodigoIntegracao { get; set; }

        [TabelaIntegracao(DisplayName = "TGFCAB.NUMNOTA")]
        [Required]
        public string Numero { get; set; } 

        [TabelaIntegracao(DisplayName = "TGFCAB.SERIENOTA")]
        public string Serie { get; set; }


        [TabelaIntegracao(DisplayName = "TGFITE.AD_QTDDEV")]
        public string QuantidadeDevolucao { get; set; }


        [TabelaIntegracao(DisplayName = "TGFCAB.CODEMP")]
        [Required]
        public string CodigoIntegracaoEmpresa { get; set; } 

        [TabelaIntegracao(DisplayName = "TGFCAB.CHAVENFE")]
        public string ChaveAcesso { get; set; } 

        [TabelaIntegracao(DisplayName = "TGFCAB.VLRNOTA")]
        [Required]
        public string ValorTotal { get; set; } 

        [TabelaIntegracao(DisplayName = "TGFCAB.CIF_FOB")]
        [Required]
        public string FreteTipo { get; set; } 

        [TabelaIntegracao(DisplayName = "TGFCAB.CODPARC")]
        [Required]
        public string CodigoIntegracaoFornecedor{ get; set; } 

        [TabelaIntegracao(DisplayName = "TGFCAB.STATUSNOTA")]
        [Required]
        public string StatusIntegracao { get; set; } 

        [TabelaIntegracao(DisplayName = "TGFCAB.CODPARCTRANSP")]
        [Required]
        public string CodigoIntegracaoTransportadora { get; set; }      

        [TabelaIntegracao(DisplayName = "TGFCAB.VLRFRETE")]
        [Required]
        public string ValorFrete { get; set; } 

        [TabelaIntegracao(DisplayName = "TGFCAB.NUMCF")]
        public string NumeroConhecimento { get; set; } 

        [TabelaIntegracao(DisplayName = "TGFCAB.PESOBRUTO")]
        public string PesoBruto { get; set; } 

        [TabelaIntegracao(DisplayName = "TGFCAB.VOLUME")]
        public string Especie { get; set; }

        [TabelaIntegracao(DisplayName = "TGFCAB.QTDVOL")] 
        [Required]
        public string QuantidadeVolume { get; set; } 

        [TabelaIntegracao(DisplayName = "TGFCAB.DTNEG")]
        public string DataEmissao { get; set; } 

        [TabelaIntegracao(DisplayName = "TGFCAB.AD_NUMFICTSAV")] 
        public string NumeroFicticioNF { get; set; } 

        [TabelaIntegracao(DisplayName = "TGFCAB.CODVEND")]
        [Required]
        public string CodigoIntegracaoVendedor { get; set; }

        [TabelaIntegracao(DisplayName = "TGFITE.CODPROD")]
        [Required]
        public string CodigoIntegracaoProduto { get; set; }

        [TabelaIntegracao(DisplayName = "TGFITE.CODVOL")]
        [Required]
        public string UnidadeMedida { get; set; }

        [TabelaIntegracao(DisplayName = "TGFITE.QTDNEG")]
        [Required]
        public string Quantidade { get; set; }

        [TabelaIntegracao(DisplayName = "TGFITE.VLRUNIT")]
        [Required]
        public string ValorUnitarioItem { get; set; }

        [TabelaIntegracao(DisplayName = "TGFITE.VLRTOT")]
        [Required]
        public string ValorTotalItem { get; set; }

        [TabelaIntegracao(DisplayName = "TGFITE.SEQUENCIA")]
        [Required]
        public string Sequencia { get; set; }
    }

    [TabelaIntegracao(DisplayName = "TGFCAN")]
    public class NotaFiscalCanceladaIntegracao
    {
        [TabelaIntegracao(DisplayName = "NUNOTA")]
        public string NUNOTA { get; set; }
    }

    [TabelaIntegracao(DisplayName = "TGFCAB")]
    public class NotaFiscalAutorizadaIntegracao
    {
        [TabelaIntegracao(DisplayName = "STATUSNFE")]
        public string StatusNFE { get; set; }
    }

    [TabelaIntegracao(DisplayName = "TGFCAB")]
    public class NotaFiscalConfirmadaIntegracao
    {
        [TabelaIntegracao(DisplayName = "TGFCAB.NUNOTA")]
        public string CodigoIntegracao { get; set; }
    }

    [TabelaIntegracao(DisplayName = "TGFFIN")]
    public class NotaFiscalDataVencimentoIntegracao
    {
        [TabelaIntegracao(DisplayName = "DTVENC")]
        public string DataVencimento { get; set; }
    }
}



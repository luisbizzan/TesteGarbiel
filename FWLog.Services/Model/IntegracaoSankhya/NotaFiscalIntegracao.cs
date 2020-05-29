using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFCAB")]
    public class NotaFiscalIntegracao
    {
        [Display(Name = "TGFCAB.NUNOTA")]
        [TabelaIntegracao(DisplayName = "TGFCAB.NUNOTA")]
        [Required]
        public string CodigoIntegracao { get; set; }

        [Display(Name = "TGFCAB.NUMNOTA")]
        [TabelaIntegracao(DisplayName = "TGFCAB.NUMNOTA")]
        [Required]
        public string Numero { get; set; }

        [Display(Name = "TGFCAB.SERIENOTA")]
        [TabelaIntegracao(DisplayName = "TGFCAB.SERIENOTA")]
        public string Serie { get; set; }

        [Display(Name = "TGFITE.AD_QTDDEV")]
        [TabelaIntegracao(DisplayName = "TGFITE.AD_QTDDEV")]
        public string QuantidadeDevolucao { get; set; }

        [Display(Name = "TGFCAB.CODEMP")]
        [TabelaIntegracao(DisplayName = "TGFCAB.CODEMP")]
        [Required]
        public string CodigoIntegracaoEmpresa { get; set; }

        [Display(Name = "TGFCAB.CHAVENFE")]
        [TabelaIntegracao(DisplayName = "TGFCAB.CHAVENFE")]
        [Required]
        public string ChaveAcesso { get; set; }

        [Display(Name = "TGFCAB.VLRNOTA")]
        [TabelaIntegracao(DisplayName = "TGFCAB.VLRNOTA")]
        [Required]
        public string ValorTotal { get; set; }

        [Display(Name = "TGFCAB.CIF_FOB")]
        [TabelaIntegracao(DisplayName = "TGFCAB.CIF_FOB")]
        [Required]
        public string FreteTipo { get; set; }

        [Display(Name = "TGFCAB.CODPARC")]
        [TabelaIntegracao(DisplayName = "TGFCAB.CODPARC")]
        [Required]
        public string CodigoIntegracaoFornecedor{ get; set; }

        [Display(Name = "TGFCAB.STATUSNOTA")]
        [TabelaIntegracao(DisplayName = "TGFCAB.STATUSNOTA")]
        [Required]
        public string StatusIntegracao { get; set; }

        [Display(Name = "TGFCAB.CODPARCTRANSP")]
        [TabelaIntegracao(DisplayName = "TGFCAB.CODPARCTRANSP")]
        [Required]
        public string CodigoIntegracaoTransportadora { get; set; }

        [Display(Name = "TGFCAB.VLRFRETE")]
        [TabelaIntegracao(DisplayName = "TGFCAB.VLRFRETE")]
        [Required]
        public string ValorFrete { get; set; }

        [Display(Name = "TGFCAB.NUMCF")]
        [TabelaIntegracao(DisplayName = "TGFCAB.NUMCF")]
        public string NumeroConhecimento { get; set; }

        [Display(Name = "TGFCAB.PESOBRUTO")]
        [TabelaIntegracao(DisplayName = "TGFCAB.PESOBRUTO")]
        public string PesoBruto { get; set; }

        [Display(Name = "TGFCAB.VOLUME")]
        [TabelaIntegracao(DisplayName = "TGFCAB.VOLUME")]
        public string Especie { get; set; }

        [Display(Name = "TGFCAB.QTDVOL")]
        [TabelaIntegracao(DisplayName = "TGFCAB.QTDVOL")] 
        [Required]
        public string QuantidadeVolume { get; set; }

        [Display(Name = "TGFCAB.DTNEG")]
        [TabelaIntegracao(DisplayName = "TGFCAB.DTNEG")]
        public string DataEmissao { get; set; }

        [Display(Name = "TGFCAB.AD_NUMFICTSAV")]
        [TabelaIntegracao(DisplayName = "TGFCAB.AD_NUMFICTSAV")] 
        public string NumeroFicticioNF { get; set; }

        [Display(Name = "TGFCAB.CODVEND")]
        [TabelaIntegracao(DisplayName = "TGFCAB.CODVEND")]
        [Required]
        public string CodigoIntegracaoVendedor { get; set; }

        [Display(Name = "TGFITE.CODPROD")]
        [TabelaIntegracao(DisplayName = "TGFITE.CODPROD")]
        [Required]
        public string CodigoIntegracaoProduto { get; set; }

        [Display(Name = "TGFITE.CODVOL")]
        [TabelaIntegracao(DisplayName = "TGFITE.CODVOL")]
        [Required]
        public string UnidadeMedida { get; set; }

        [Display(Name = "TGFITE.QTDNEG")]
        [TabelaIntegracao(DisplayName = "TGFITE.QTDNEG")]
        [Required]
        public string Quantidade { get; set; }

        [Display(Name = "TGFITE.VLRUNIT")]
        [TabelaIntegracao(DisplayName = "TGFITE.VLRUNIT")]
        [Required]
        public string ValorUnitarioItem { get; set; }

        [Display(Name = "TGFITE.VLRTOT")]
        [TabelaIntegracao(DisplayName = "TGFITE.VLRTOT")]
        [Required]
        public string ValorTotalItem { get; set; }

        [Display(Name = "TGFITE.SEQUENCIA")]
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



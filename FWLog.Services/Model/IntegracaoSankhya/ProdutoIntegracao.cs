using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFPRO")]
    public class ProdutoIntegracao
    {
        [Display(Name = "TGFPRO.CODPROD")]
        [TabelaIntegracao(DisplayName = "CODPROD")]
        [Required]
        public string CodigoIntegracao { get; set; }

        [Display(Name = "TGFPRO.DESCRPROD")]
        [TabelaIntegracao(DisplayName = "DESCRPROD")]
        [Required]
        public string Descricao { get; set; }

        [Display(Name = "TGFPRO.CODFAB")]
        [TabelaIntegracao(DisplayName = "CODFAB")]
        public string CodigoFabricante { get; set; }

        [Display(Name = "TGFPRO.FABRICANTE")]
        [TabelaIntegracao(DisplayName = "FABRICANTE")]
        public string NomeFabricante { get; set; }

        [Display(Name = "TGFPRO.LARGURA")]
        [TabelaIntegracao(DisplayName = "LARGURA")]
        public string Largura { get; set; }

        [Display(Name = "TGFPRO.ALTURA")]
        [TabelaIntegracao(DisplayName = "ALTURA")]
        public string Altura { get; set; }

        [Display(Name = "TGFPRO.ATIVO")]
        [TabelaIntegracao(DisplayName = "ATIVO")]
        [Required]
        public string Ativo { get; set; }

        [Display(Name = "TGFPRO.ENDIMAGEM")]
        [TabelaIntegracao(DisplayName = "ENDIMAGEM")]
        public string EnderecoImagem { get; set; }

        [Display(Name = "TGFPRO.ESPESSURA")]
        [TabelaIntegracao(DisplayName = "ESPESSURA")]
        public string Comprimento { get; set; }

        [Display(Name = "TGFPRO.PRODUTONFE")]
        [TabelaIntegracao(DisplayName = "PRODUTONFE")]
        [Required]
        public string CodigoProdutoNFE { get; set; }

        [Display(Name = "TGFPRO.M3")]
        [TabelaIntegracao(DisplayName = "M3")]
        public string MetroCubico { get; set; }

        [Display(Name = "TGFPRO.AGRUPMIN")]
        [TabelaIntegracao(DisplayName = "AGRUPMIN")]
        [Required]
        public string MultiploVenda { get; set; }

        [Display(Name = "TGFPRO.PESOBRUTO")]
        [TabelaIntegracao(DisplayName = "PESOBRUTO")]
        [Required]
        public string PesoBruto { get; set; }

        [Display(Name = "TGFPRO.PESOLIQ")]
        [TabelaIntegracao(DisplayName = "PESOLIQ")]
        [Required]
        public string PesoLiquido { get; set; }

        [Display(Name = "TGFPRO.AD_CODBAR")]
        [TabelaIntegracao(DisplayName = "AD_CODBAR")]
        public string CodigoBarras { get; set; }

        [Display(Name = "TGFPRO.REFERENCIA")]
        [TabelaIntegracao(DisplayName = "REFERENCIA")]
        public string CodigoBarras2 { get; set; }

        [Display(Name = "TGFPRO.AD_REFX")]
        [TabelaIntegracao(DisplayName = "AD_REFX")]
        public string Referencia { get; set; }

        [Display(Name = "TGFPRO.REFFORN")]
        [TabelaIntegracao(DisplayName = "REFFORN")]
        public string ReferenciaFornecedor { get; set; }

        [Display(Name = "TGFPRO.CODVOL")]
        [TabelaIntegracao(DisplayName = "CODVOL")]
        public string UnidadeMedidaSigla { get; set; }

        [TabelaIntegracao(DisplayName = "AD_EMBFORNEC")]
        public string IsEmbalagemFornecedor { get; set; }

        [TabelaIntegracao(DisplayName = "AD_EMBFORNECVOL")]
        public string IsEmbalagemFornecedorVolume { get; set; }

    }

    [TabelaIntegracao(DisplayName = "TGFPRO")]
    public class ProdutoQuantidadeRegistroIntegracao
    {
        [TabelaIntegracao(DisplayName = "COUNT(1)")]
        public string QuantidadeRegistro { get; set; }
    }
}

using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFPRO")]
    public class ProdutoIntegracao
    {
        [TabelaIntegracao(DisplayName = "CODPROD")]
        [Required]
        public string CodigoIntegracao { get; set; }

        [TabelaIntegracao(DisplayName = "DESCRPROD")]
        [Required]
        public string Descricao { get; set; }

        [TabelaIntegracao(DisplayName = "CODFAB")]
        public string CodigoFabricante { get; set; }

        [TabelaIntegracao(DisplayName = "FABRICANTE")]
        public string NomeFabricante { get; set; }

        [TabelaIntegracao(DisplayName = "LARGURA")]
        public string Largura { get; set; }

        [TabelaIntegracao(DisplayName = "ALTURA")]
        public string Altura { get; set; }
               
        [TabelaIntegracao(DisplayName = "ATIVO")]
        [Required]
        public string Ativo { get; set; }

        [TabelaIntegracao(DisplayName = "ENDIMAGEM")]
        public string EnderecoImagem { get; set; }

        [TabelaIntegracao(DisplayName = "ESPESSURA")]
        public string Comprimento { get; set; }

        [TabelaIntegracao(DisplayName = "PRODUTONFE")]
        [Required]
        public string CodigoProdutoNFE { get; set; }

        [TabelaIntegracao(DisplayName = "M3")]
        public string MetroCubico { get; set; }

        [TabelaIntegracao(DisplayName = "AGRUPCOMPMINIMO")]
        [Required]
        public string MultiploVenda { get; set; }

        [TabelaIntegracao(DisplayName = "PESOBRUTO")]
        [Required]
        public string PesoBruto { get; set; }

        [TabelaIntegracao(DisplayName = "PESOLIQ")]
        [Required]
        public string PesoLiquido { get; set; }

        [TabelaIntegracao(DisplayName = "AD_CODBAR")]
        public string CodigoBarras { get; set; }

        [TabelaIntegracao(DisplayName = "REFERENCIA")]
        public string CodigoBarras2 { get; set; }

        [TabelaIntegracao(DisplayName = "AD_REFX")]
        public string Referencia { get; set; }

        [TabelaIntegracao(DisplayName = "REFFORN")]
        public string ReferenciaFornecedor { get; set; }

        [TabelaIntegracao(DisplayName = "CODVOL")]
        public string UnidadeMedidaSigla { get; set; }

    }
}

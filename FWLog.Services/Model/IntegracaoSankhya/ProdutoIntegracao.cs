using FWLog.Services.Integracao.Helpers;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFPRO")]
    public class ProdutoIntegracao
    {
        public string CODPROD { get; set; }
        public string DESCRPROD { get; set; } 
        public string CODFAB { get; set; } 
        public string DESCRPRODNFE { get; set; } 
        public string FABRICANTE { get; set; } 
        public string LARGURA { get; set; } 
        public string ALTURA { get; set; } 
        public string UNIDADE { get; set; } 
        public string ATIVO { get; set; } 
        public string ENDIMAGEM { get; set; } 
        public string ESPESSURA { get; set; } 
        public string PRODUTONFE { get; set; } 
        public string M3 { get; set; } 
        public string MULTIPVENDA { get; set; } 
        public string PESOBRUTO { get; set; } 
        public string PESOLIQ { get; set; } 
        public string REFERENCIA { get; set; } 
        public string AD_REFX { get; set; } 
        public string REFFORN { get; set; } 
    }
}

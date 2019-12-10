using FWLog.Services.Integracao.Helpers;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TDDOPC")]
    public class FreteTipoIntegracao
    {
        public string VALOR { get; set; }
        public string OPCAO { get; set; }
    }
}

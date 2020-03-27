namespace FWLog.Web.Api.Models.Armazenagem
{
    public class NivelPontoArmazenagemPorCorredorResposta
    {
        public long IdNivelArmazenagem { get; set; }
        public long IdPontoArmazenagem { get; set; }
        public string NivelArmazenagemDescricao { get; set; }
        public string PontoArmazenagemDescricao { get; set; }
    }
}
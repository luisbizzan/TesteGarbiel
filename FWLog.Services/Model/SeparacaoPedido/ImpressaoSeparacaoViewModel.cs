using FWLog.Services.Model.GrupoCorredorArmazenagem;

namespace FWLog.Services.Model.SeparacaoPedido
{
    public class ImpressaoSeparacaoViewModel
    {
        public VolumeViewModel Volume { get; set; }
        public int NumeroVolume { get; set; }
        public GrupoCorredorArmazenagemViewModel GrupoCorredor { get; set; }
        public int Centena { get; set; }
        public int CorredorinicioSeparacao { get; set; }
    }
}

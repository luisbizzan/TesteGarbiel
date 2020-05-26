namespace FWLog.Data.Models.FilterCtx
{
    public class MovimentacaoVolumesModel
    {
        public string Corredores { get; set; }

        public string PontoArmazenagemDescricao { get; set; }

        public int EnviadoSeparacao { get; set; }

        public int EmSeparacao { get; set; }

        public int FinalizadoSeparacao { get; set; }

        public int InstaladoTransportadora { get; set; }

        public int Doca { get; set; }

        public int EnviadoTransportadora { get; set; }

        public int Total { get; set; }
    }
}
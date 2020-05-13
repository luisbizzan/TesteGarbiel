namespace FWLog.Services.Model.GrupoCorredorArmazenagem
{
    public class GrupoCorredorArmazenagemViewModel
    {
        public long IdGrupoCorredorArmazenagem { get; set; }

        public long IdEmpresa { get; set; }

        public int CorredorInicial { get; set; }

        public int CorredorFinal { get; set; }

        public long IdPontoArmazenagem { get; set; }

        public long IdImpressora { get; set; }

        public bool Ativo { get; set; }
    }
}

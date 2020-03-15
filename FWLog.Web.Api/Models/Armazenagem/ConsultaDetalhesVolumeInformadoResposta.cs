using System;

namespace FWLog.Web.Api.Models.Armazenagem
{
    public class ConsultaDetalhesVolumeInformadoResposta
    {
        public int Quantidade { get; set; }

        public decimal PesoTotal { get; set; }

        public decimal? LimitePeso { get; set; }

        public string IdUsuarioInstalacao { get; set; }

        public DateTime DataHoraInstalacao { get; set; }
    }
}
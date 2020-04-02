using FWLog.Data.Models;

namespace FWLog.Services.Model.Coletor
{
    public class GravarHistoricoColetorRequisicao
    {
        public long IdEmpresa { get; set; }
        public ColetorAplicacaoEnum IdColetorAplicacao { get; set; }
        public ColetorHistoricoTipoEnum IdColetorHistoricoTipo { get; set; }
        public string IdUsuario { get; set; }
        public string Descricao { get; set; }
    }
}

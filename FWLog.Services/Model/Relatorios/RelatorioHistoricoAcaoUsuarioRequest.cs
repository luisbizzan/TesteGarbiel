using System;

namespace FWLog.Services.Model.Relatorios
{
    public class RelatorioHistoricoAcaoUsuarioRequest
    {
        public long? IdEmpresa { get; set; }
        public string NomeUsuarioRequisicao { get; set; }
        public int? IdColetorAplicacao { get; set; }
        public int? IdHistoricoColetorTipo { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public string IdUsuario { get; set; }
        public string ColetorAplicacao { get; set; }
        public string HistoricoColetorTipo { get; set; }
        public string UsuarioSelecionado { get; set; }
    }
}

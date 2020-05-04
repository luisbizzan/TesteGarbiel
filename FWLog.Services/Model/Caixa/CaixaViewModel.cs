using FWLog.Data.Models;

namespace FWLog.Services.Model.Caixa
{
    public class CaixaViewModel
    {
        public long IdCaixa { get; set; }
        public CaixaTipoEnum IdCaixaTipo { get; set; }
        public string Nome { get; set; }
        public string TextoEtiqueta { get; set; }
        public decimal Largura { get; set; }
        public decimal Altura { get; set; }
        public decimal Comprimento { get; set; }
        public decimal Cubagem { get; set; }
        public decimal PesoCaixa { get; set; }
        public decimal PesoMaximo { get; set; }
        public decimal Sobra { get; set; }
        public int Prioridade { get; set; }
        public bool Ativo { get; set; }
        public int QuantidadeRanking { get; set; }
    }
}

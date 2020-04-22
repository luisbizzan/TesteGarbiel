using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class CaixaListaTabela
    {
        public long IdCaixa { get; set; }

        [Display(Name = "Nome da Caixa")]
        public string Nome { get; set; }

        [Display(Name = "Texto Etiqueta")]
        public string TextoEtiqueta { get; set; }

        [Display(Name = "Largura")]
        public decimal Largura { get; set; }

        [Display(Name = "Altura")]
        public decimal Altura { get; set; }

        [Display(Name = "Comprimento")]
        public decimal Comprimento { get; set; }

        [Display(Name = "Peso Máximo")]
        public decimal PesoMaximo { get; set; }

        [Display(Name = "Cubicagem")]
        public decimal Cubagem { get; set; }

        [Display(Name = "Sobra")]
        public decimal Sobra { get; set; }

        [Display(Name = "Caixa para")]
        public CaixaTipoEnum IdCaixaTipo { get; set; }

        [Display(Name = "Peso da Caixa")]
        public decimal PesoCaixa { get; set; }

        [Display(Name = "Prioridade")]
        public int Prioridade { get; set; }

        [Display(Name = "Status da Caixa")]
        public string Status { get; set; }
    }
}
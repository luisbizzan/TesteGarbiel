using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models.FilterCtx
{
    public class CaixaListaFiltro
    {
        public long IdEmpresa { get; set; }

        [Display(Name = "Nome da Caixa")]
        public string Nome { get; set; }

        [Display(Name = "Texto Etiqueta")]
        public string TextoEtiqueta { get; set; }

        [Display(Name = "Peso Máximo")]
        public decimal? PesoMaximo { get; set; }

        [Display(Name = "Cubicagem")]
        public decimal? Cubagem { get; set; }

        [Display(Name = "Sobra")]
        public decimal? Sobra { get; set; }

        [Display(Name = "Caixa para")]
        public CaixaTipoEnum? IdCaixaTipo { get; set; }

        [Display(Name = "Peso da Caixa")]
        public decimal? PesoCaixa { get; set; }

        [Display(Name = "Prioridade")]
        public decimal? Prioridade { get; set; }

        [Display(Name = "Status da Caixa")]
        public bool? Status { get; set; }
    }
}
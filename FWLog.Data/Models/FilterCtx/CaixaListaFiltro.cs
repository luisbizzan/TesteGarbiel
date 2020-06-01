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

        [Display(Name = "Peso Máximo (Kg)")]
        public decimal? PesoMaximo { get; set; }

        [Display(Name = "Cubicagem (CM³)")]
        public decimal? Cubagem { get; set; }

        [Display(Name = "Sobra (%)")]
        public decimal? Sobra { get; set; }

        [Display(Name = "Caixa para")]
        public CaixaTipoEnum? IdCaixaTipo { get; set; }

        [Display(Name = "Peso da Caixa (Kg)")]
        public decimal? PesoCaixa { get; set; }

        [Display(Name = "Status da Caixa")]
        public bool? Status { get; set; }
    }
}
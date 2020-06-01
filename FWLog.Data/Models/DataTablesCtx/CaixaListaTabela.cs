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

        [Display(Name = "Largura (CM)")]
        public decimal Largura { get; set; }

        [Display(Name = "Altura (CM)")]
        public decimal Altura { get; set; }

        [Display(Name = "Comprimento (CM)")]
        public decimal Comprimento { get; set; }

        [Display(Name = "Peso Máximo (Kg)")]
        public decimal PesoMaximo { get; set; }

        [Display(Name = "Cubagem (CM³)")]
        public decimal Cubagem { get; set; }

        [Display(Name = "Sobra (%)")]
        public decimal Sobra { get; set; }

        [Display(Name = "Caixa para")]
        public string CaixaTipoDescricao { get; set; }

        [Display(Name = "Peso (Kg)")]
        public decimal PesoCaixa { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }
    }
}
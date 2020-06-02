using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.CaixaCtx
{
    public class CaixaDetalhesViewModel
    {
        public long IdCaixa { get; set; }

        [Display(Name = "Caixa para")]
        public string CaixaTipoDescricao { get; set; }

        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Display(Name = "Texto Etiqueta")]
        public string TextoEtiqueta { get; set; }

        [Display(Name = "Largura (CM)")]
        public decimal Largura { get; set; }

        [Display(Name = "Altura (CM)")]
        public decimal Altura { get; set; }

        [Display(Name = "Cubagem (CM³)")]
        public decimal Cubagem { get; set; }

        [Display(Name = "Comprimento (CM)")]
        public decimal Comprimento { get; set; }

        [Display(Name = "Peso Caixa (Kg)")]
        public string PesoCaixa { get; set; }

        [Display(Name = "Peso Máximo (Kg)")]
        public string PesoMaximo { get; set; }

        [Display(Name = "Sobra (%)")]
        public string Sobra { get; set; }

        [Display(Name = "Ativo")]
        public string Ativo { get; set; }
    }
}
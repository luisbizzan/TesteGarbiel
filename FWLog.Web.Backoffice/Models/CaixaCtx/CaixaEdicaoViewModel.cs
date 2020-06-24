using FWLog.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.CaixaCtx
{
    [Bind(Exclude = "ListaCaixaTipo")]
    public class CaixaEdicaoViewModel
    {
        [Required]
        public long IdCaixa { get; set; }

        [Display(Name = "Caixa para")]
        [Required]
        public CaixaTipoEnum? IdCaixaTipo { get; set; }

        [Display(Name = "Nome")]
        [Required]
        [StringLength(50)]
        public string Nome { get; set; }

        [Display(Name = "Texto Etiqueta")]
        [Required]
        [StringLength(50)]
        public string TextoEtiqueta { get; set; }

        [Display(Name = "Largura (CM)")]
        [Required]
        public string Largura { get; set; }

        [Display(Name = "Altura (CM)")]
        [Required]
        public string Altura { get; set; }

        [Display(Name = "Comprimento (CM)")]
        [Required]
        public string Comprimento { get; set; }

        [Display(Name = "Peso Caixa (Kg)")]
        [Required]
        public string PesoCaixa { get; set; }

        [Display(Name = "Peso Máximo (Kg)")]
        [Required]
        public string PesoMaximo { get; set; }

        [Display(Name = "Sobra (%)")]
        [Required]
        [Range(0, 100)]
        public decimal? Sobra { get; set; }

        [Display(Name = "Ativo")]
        [Required]
        public bool Ativo { get; set; }

        public SelectList ListaCaixaTipo { get; set; }
    }
}
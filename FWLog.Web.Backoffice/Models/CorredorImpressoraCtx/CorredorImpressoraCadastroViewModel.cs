using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.CorredorImpressoraCtx
{
    public class CorredorImpressoraCadastroViewModel
    {
        public long IdEmpresa { get; set; }
        [Display(Name = "Corredor Inicial")]
        [Required]
        public int? CorredorInicial { get; set; }

        [Display(Name = "Corredor Final")]
        [Required]
        public int? CorredorFinal { get; set; }

        [Display(Name = "Ponto de Armazenagem")]
        public long IdPontoArmazenagem { get; set; }

        [Required]
        public string DescricaoPontoArmazenagem { get; set; }

        [Display(Name = "Impressora")]
        [Required]
        public long IdImpressora { get; set; }

        [Display(Name = "Status")]
        [Required]
        public bool Ativo { get; set; }

        public SelectList ListaImpressora { get; set; }
    }
}
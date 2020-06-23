using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.CorredorImpressoraCtx
{
    public class CorredorImpressoraDetalhesViewModel
    {
        public long IdGrupoCorredorArmazenagem { get; set; }

        public long IdEmpresa { get; set; }
        [Display(Name = "Corredor Inicial")]
        [Required]
        public string CorredorInicial { get; set; }

        [Display(Name = "Corredor Final")]
        [Required]
        public string CorredorFinal { get; set; }

        [Display(Name = "Ponto de Armazenagem")]
        [Required]
        public string DescricaoPontoArmazenagem { get; set; }

        [Display(Name = "Impressora")]
        [Required]
        public string DescricaoImpressora { get; set; }

        [Display(Name = "Impressora Filial")]
        [Required]
        public string DescricaoImpressoraPedidoFilial { get; set; }

        [Display(Name = "Ativo")]
        [Required]
        public string Ativo { get; set; }
    }
}
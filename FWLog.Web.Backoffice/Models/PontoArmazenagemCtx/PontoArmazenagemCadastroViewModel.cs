using FWLog.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.PontoArmazenagemCtx
{
    [Bind(Exclude = "DescricaoNivelArmazenagem,TiposArmazenagem,TiposMovimentacao")]
    public class PontoArmazenagemCadastroViewModel
    {
        [Required]
        [Display(Name = "Nível de Armazenagem")]
        public long? IdNivelArmazenagem { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Ponto de Armazenagem")]
        public string Descricao { get; set; }
        [Required]
        [Display(Name = "Tipo de Armazenagem")]
        public TipoArmazenagemEnum IdTipoArmazenagem { get; set; }
        [Required]
        [Display(Name = "Tipo de Movimentação")]
        public TipoMovimentacaoEnum IdTipoMovimentacao { get; set; }
        [Display(Name = "Limite de Peso Vertical (KG)")]
        public decimal? LimitePesoVertical { get; set; }
        [Required]
        [Display(Name = "Status")]
        public bool Ativo { get; set; }

        public string DescricaoNivelArmazenagem { get; set; }
        public SelectList TiposArmazenagem { get; set; }
        public SelectList TiposMovimentacao { get; set; }
    }
}
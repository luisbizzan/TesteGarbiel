using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.EnderecoArmazenagemCtx
{
    public class EnderecoArmazenagemEditarViewModel
    {
        [Required]
        public long IdEnderecoArmazenagem { get; set; }
        [Required]
        [Display(Name = "Nível de Armazenagem")]
        public long? IdNivelArmazenagem { get; set; }
        [Required]
        [Display(Name = "Ponto de Armazenagem")]
        public long? IdPontoArmazenagem { get; set; }
        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "O código do endereço informado não é válido")]
        [Display(Name = "Código Endereço")]
        public string Codigo { get; set; }
        [Required]
        [Display(Name = "Controla FIFO?")]
        public bool IsFifo { get; set; }
        [Display(Name = "Limite de Peso")]
        public decimal? LimitePeso { get; set; }
        [Required]
        [Display(Name = "Ponto de Separação?")]
        public bool IsPontoSeparacao { get; set; }
        [Display(Name = "Estoque Mínimo")]
        public int? EstoqueMinimo { get; set; }
        [Display(Name = "Estoque Máximo")]
        public int? EstoqueMaximo { get; set; }
        [Required]
        [Display(Name = "Entrada?")]
        public bool IsEntrada { get; set; }
        [Display(Name = "Picking?")]
        public bool IsPkicing { get; set; }
        [Required]
        [Display(Name = "Ativo?")]
        public bool Ativo { get; set; }

        public string DescricaoNivelArmazenagem { get; set; }
        public string DescricaoPontoArmazenagem { get; set; }
    }
}
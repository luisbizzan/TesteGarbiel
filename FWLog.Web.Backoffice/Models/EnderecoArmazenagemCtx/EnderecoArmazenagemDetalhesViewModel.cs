using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.EnderecoArmazenagemCtx
{
    public class EnderecoArmazenagemDetalhesViewModel
    {
        [Display(Name = "Código")]
        public long IdEnderecoArmazenagem { get; set; }
        [Display(Name = "Nível de Armazenagem")]
        public string NivelArmazenagem { get; set; }
        [Display(Name = "Ponto de Armazenagem")]
        public string PontoArmazenagem { get; set; }
        [Display(Name = "Endereco Armazenagem")]
        public string Codigo { get; set; }
        [Display(Name = "Estoque Minimo")]
        public int? EstoqueMinimo { get; set; }
        [Display(Name = "Estoque Máximo")]
        public int? EstoqueMaximo { get; set; }
        [Display(Name = "Limite de Peso")]
        public decimal LimitePeso { get; set; }
        [Display(Name = "Ponto de Separação?")]
        public string PontoSeparacao { get; set; }
        [Display(Name = "Controla FIFO?")]
        public string Fifo { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
    }
}
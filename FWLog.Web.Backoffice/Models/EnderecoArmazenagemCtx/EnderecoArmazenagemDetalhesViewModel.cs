using System.Collections.Generic;
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
        [Display(Name = "Entrada?")]
        public string IsEntrada { get; set; }
        [Display(Name = "Picking?")]
        public string IsPicking { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }

        public List<ProdutoItem> Items { get; set; }

        public EnderecoArmazenagemDetalhesViewModel()
        {
            Items = new List<ProdutoItem>();
        }
    }

    public class ProdutoItem
    {
        public string NumeroNf { get; set; }
        public string NumeroLote { get; set; }
        public string CodigoReferencia { get; set; }
        public string Descricao { get; set; }
        public string Multiplo { get; set; }
        public string QuantidadeInstalada { get; set; }
        public string Peso { get; set; }
        public string DataInstalacao { get; set; }
        public string UsuarioResponsavel { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class BOEntradaConferenciaViewModel
    {
        [Display(Name = "Lote")]
        public long IdLote { get; set; }

        [Display(Name = "Nota Fiscal")]
        public string NumeroNotaFiscal { get; set; }
        public long IdNotaFiscal { get; set; }

        [Display(Name = "Conferente")]
        public string NomeConferente { get; set; }
        public string IdUuarioConferente { get; set; }

        [Display(Name = "Fornecedor")]
        public string NomeFornecedor { get; set; }
        
        [Display(Name = "Recebido em")]
        public string DataHoraRecebimento { get; set; }

        [Display(Name = "Qtde. Volumes")]
        public long QuantidadeVolume { get; set; }

        [Display(Name = "Referência")]
        public string Referencia { get; set; }

        [Display(Name = "Embalagem")]
        public string Embalagem { get; set; }

        [Display(Name = "Unidade")]
        public string Unidade { get; set; }

        [Display(Name = "Múltiplo")]
        public string Multiplo { get; set; }

        [Display(Name = "Qtde. por Caixa")]
        public int? QuantidadePorCaixa { get; set; }

        [Display(Name = "Qtde. de Caixas")]
        public int? QuantidadeCaixa { get; set; }

        [Display(Name = "Total de Itens")]
        public int? TotalItens { get; set; }

        [Display(Name = "Média de Vendas")]
        public decimal MediaVendaMes { get; set; }

        [Display(Name = "Qtde. Estoque")]
        public int? Estoque { get; set; }

        [Display(Name = "Localização")]
        public string Localizacao { get; set; }

        [Display(Name = "Qtde. Não Conferida")]
        public int? QuantidadeNaoConferida { get; set; }

        [Display(Name = "Qtde. Conferida")]
        public int? QuantidadeConferida { get; set; }

         [Display(Name = "Qtde. Reservada")]
        public int? QuantidadeReservada { get; set; }

        public string EnviarPara { get; set; }

    }
}
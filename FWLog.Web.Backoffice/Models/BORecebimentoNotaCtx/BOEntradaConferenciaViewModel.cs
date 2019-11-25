using System;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class BOEntradaConferenciaViewModel
    {
        [Display(Name = "Fornecedor")]
        public string NomeFornecedor { get; set; }

        [Display(Name = "Número Nota Fiscal")]
        public string NumeroNotaFiscal { get; set; }

        [Display(Name = "Recebimento")]
        public string DataHoraRecebimento { get; set; }

        [Display(Name = "Quantidade de Volumes")]
        public int QuantidadeVolumes { get; set; }

        [Display(Name = "Referência")]
        public string Referencia { get; set; }

        [Display(Name = "Embalagem")]
        public string Embalagem { get; set; }

        [Display(Name = "Unidade")]
        public string Unidade { get; set; }

        [Display(Name = "Multiplo")]
        public string Multiplo { get; set; }

        [Display(Name = "Quantidade por Volume")]
        public int QuantidadePorVolumes { get; set; }

        [Display(Name = "Número de Caixas")]
        public int NumerosDeCaixas { get; set; }

        [Display(Name = "Total de Itens")]
        public int TotalItens { get; set; }

        [Display(Name = "Média de Vendas")]
        public decimal MediaVendaMes { get; set; }

        [Display(Name = "Estoque")]
        public int Estoque { get; set; }

        [Display(Name = "Localização")]
        public string Localizacao { get; set; }

        [Display(Name = "Quantidade Não Conferida")]
        public int QuantidadeNaoConferida { get; set; }

        [Display(Name = "Quantidade Conferida")]
        public int QuantidadeConferida { get; set; }

        [Display(Name = "Quantidade por Porta Palete")]
        public int QuantidadePortaPalete { get; set; }

        [Display(Name = "Reserva Vendas")]
        public int ReservaVendas { get; set; }

        [Display(Name = "Apoio")]
        public int Apoio { get; set; }

        [Display(Name = "Conferente")]
        public long? IdConferente { get; set; }
        public string NomeConferente { get; set; }

        public long IdNotaFiscal { get; set; }
        //public string NumeroSerieNotaFiscal { get; set; }
        //public string HoraRecebimento { get; set; }
        //public string DataRecebimento { get; set; }
        //public DateTime DataAtual { get; set; }
        //public string FornecedorNome { get; set; }
        //public string ChaveAcesso { get; set; }
        //public string ValorTotal { get; set; }
        //public string ValorFrete { get; set; }
        //public long? NumeroConhecimento { get; set; }
        //public int? QtdVolumes { get; set; }
        //public string Peso { get; set; }
        //public string TransportadoraNome { get; set; }
        //public bool NotaFiscalPesquisada { get; set; }

    }
}
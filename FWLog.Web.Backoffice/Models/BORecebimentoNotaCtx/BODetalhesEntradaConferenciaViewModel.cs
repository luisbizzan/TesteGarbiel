using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class BODetalhesEntradaConferenciaViewModel
    {
        public long IdNotaFiscal { get; set; }
        [Display(Name = "Chave Acesso")]
        public string ChaveAcesso { get; set; }
        [Display(Name = "Lote")]
        public string NumeroLote { get; set; }
        [Display(Name = "Nota")]
        public string NumeroNotaFiscal { get; set; }
        [Display(Name = "Chegada")]
        public string DataChegada { get; set; }
        [Display(Name = "Status")]
        public string StatusNotaFiscal { get; set; }
        [Display(Name = "Fornecedor")]
        public string Fornecedor { get; set; }
        [Display(Name = "Qtd. Peças")]
        public string QuantidadePeca { get; set; }
        [Display(Name = "Atraso")]
        public string DiasAtraso { get; set; }
        [Display(Name = "Compras")]
        public string DataCompra { get; set; }
        [Display(Name = "Volumes")]
        public string Volumes { get; set; }
        [Display(Name = "Prazo")]
        public string PrazoRecebimento { get; set; }
        [Display(Name = "CNPJ")]
        public string FornecedorCNPJ { get; set; }
        [Display(Name = "Recebido por")]
        public string UsuarioRecebimento { get; set; }
        [Display(Name = "Valor Total")]
        public string ValorTotal { get; set; }
        [Display(Name = "Frete")]
        public string ValorFrete { get; set; }
        [Display(Name = "Nro. CT-e")]
        public string NumeroConhecimento { get; set; }
        [Display(Name = "Peso Conhecimento")]
        public string PesoConhecimento { get; set; }
        [Display(Name = "Transportadora")]
        public string TransportadoraNome { get; set; }
        [Display(Name = "Tipo de Conferência")]
        public string ConferenciaTipo { get; set; }
        [Display(Name = "Conferido por")]
        public string UsuarioConferencia { get; set; }
        [Display(Name = "Início")]
        public string DataInicioConferencia { get; set; }
        [Display(Name = "Fim")]
        public string DataFimConferencia { get; set; }
        [Display(Name = "Tempo Total")]
        public string TempoTotalConferencia { get; set; }
        [Display(Name = "Observação")]
        public string Observacao { get; set; }
        public bool EmConferenciaOuConferido { get; set; }
        public bool Finalizado { get; set; } = false;
        public bool ConferidoDivergencia  { get; set; } = false;

        public List<BODetalhesEntradaConferenciaItem> Items { get; set; }

        public ExibirDivergenciaRecebimentoViewModel Divergencias { get; set; }

        public BODetalhesEntradaConferenciaViewModel()
        {
            Items = new List<BODetalhesEntradaConferenciaItem>();
            Divergencias = new ExibirDivergenciaRecebimentoViewModel();
        }
    }

    public class BODetalhesEntradaConferenciaItem
    {
        public string Referencia { get; set; }
        public string DescricaoProduto { get; set; }
        public long Quantidade { get; set; }
        public string DataInicioConferencia { get; set; }
        public string DataFimConferencia { get; set; }
        public string UsuarioConferencia { get; set; }
        public string TempoConferencia { get; set; }
    }
}
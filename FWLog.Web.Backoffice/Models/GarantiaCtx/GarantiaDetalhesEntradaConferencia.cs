using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.GarantiaCtx
{
    public class GarantiaDetalhesEntradaConferencia
    {
        public List<GarantiaDetalhesEntradaConferenciaItem> ItensDaNota { get; set; }

        public GarantiaDetalhesEntradaConferencia()
        {
            ItensDaNota = new List<GarantiaDetalhesEntradaConferenciaItem>();
        }

        public long IdNotaFiscal { get; set; }
        [Display(Name = "Chave Acesso")]
        public string ChaveAcesso { get; set; }
        [Display(Name = "Nº Nota")]
        public string NumeroNotaFiscal { get; set; }
        [Display(Name = "Nº Fictício(SAV)")]
        public string NroFicticio { get; set; }
        [Display(Name = "Data Emissão da NF")]
        public string DataEmissaoNF { get; set; }
        [Display(Name = "CNPJ do Cliente")]
        public string CienteCNPJ { get; set; }
        [Display(Name = "Razão Social(Cliente)")]
        public string RazaoSocialCliente { get; set; }
        [Display(Name = "Base ICMS")]
        public string BaseICMS { get; set; }
        [Display(Name = "Valor ICMS")]
        public string ValorICMS { get; set; }
        [Display(Name = "Base ST")]
        public string BaseST { get; set; }
        [Display(Name = "Valor ST")]
        public string ValorST { get; set; }
        [Display(Name = "Valor IPI")]
        public string ValorIPI { get; set; }
        [Display(Name = "Valor Produtos")]
        public string ValorProduto { get; set; }
        [Display(Name = "Valor Frete")]
        public string ValorFrete { get; set; }
        [Display(Name = "Valor Seguro")]
        public string ValorSeguro { get; set; }
        [Display(Name = "Valor Total")]
        public string ValorTotal { get; set; }
    }

    public class GarantiaDetalhesEntradaConferenciaItem
    {
        public string Referencia { get; set; }
        public string DescricaoProduto { get; set; }
        public string Unidade { get; set; }
        public string CFOP { get; set; }
        public long QuantidadeDeProduto { get; set; }
        public string NumeroNotaFiscalOrigem { get; set; }
    }
}
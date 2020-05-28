using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class ExibirDevolucaoTotalViewModel
    {
        public ExibirDevolucaoTotalViewModel()
        {
            Processamento = new ProcessamentoDevolucaoTotalViewModel();
        }
        
        public long IdLote { get; set; }

        [Display(Name = "Nota Fiscal")]
        public string NotaFiscal { get; set; }
        
        [Display(Name = "Status")]
        public string StatusNotasFiscal { get; set; }
        public int QuantidadeEtiqueta { get; set; }
        public ProcessamentoDevolucaoTotalViewModel Processamento { get; set; }
    }

    public class ProcessamentoDevolucaoTotalViewModel
    {
        [Display(Name = "Atualização do Status de Recebimento da Nota Fiscal de Compra")]
        public bool AtualizacaoNFCompra { get; set; }
        
        [Display(Name = "Confirmação da Nota Fiscal de Compra no Sankhya")]
        public bool ConfirmacaoNFCompra { get; set; }
        
        [Display(Name = "Criação da Quarentena")]
        public bool CriacaoQuarentena { get; set; }
        
        [Display(Name = "Criação da Nota Fiscal de Devolução")]
        public bool CriacaoNFDevolucao { get; set; }
        
        [Display(Name = "Confirmação da Nota Fiscal de Devolução")]
        public bool ConfirmacaoNFDevolucao { get; set; }
        
        [Display(Name = "Autorização da Nota Fiscal de Devolução no Sefaz")]
        public bool AutorizacaoNFDevolucaoSefaz { get; set; }
    }
}
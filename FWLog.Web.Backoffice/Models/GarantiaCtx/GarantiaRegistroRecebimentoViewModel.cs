using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.GarantiaCtx
{
    public class GarantiaRegistroRecebimentoViewModel
    {
        [Display(Name = "Chave Acesso")]
        public string ChaveAcesso { get; set; }
        public long IdNotaFiscal { get; set; }
        [Display(Name = "Nº Nota Fiscal")]
        public long? NumeroNF { get; set; }
        [Display(Name = "Observação")]
        [StringLength(500, ErrorMessage = "Observação deve conter no máximo 500 caracteres.")]
        public string Observacao { get; set; }
        [Display(Name = "Informações de Transporte")]
        public string InformacaoTransporte { get; set; }
    }
}
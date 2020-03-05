using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.GarantiaCtx
{
    public class GarantiaEntradaConferenciaViewModel
    {
        [Display(Name = "Nº Solicitação")]
        public long IdGarantia { get; set; }

        [Display(Name = "Nota Fiscal")]
        public string NumeroNotaFiscal { get; set; }
        public long IdNotaFiscal { get; set; }

        [Display(Name = "Data Solicitação")]
        public string DataDaSolicitacao { get; set; }

        [Display(Name = "Representante")]
        public string Representante { get; set; }

        [Display(Name = "Cliente")]
        public string Cliente { get; set; }

        [Display(Name = "Conferente")]
        public string NomeConferente { get; set; }
        public string IdUuarioConferente { get; set; }

        [Display(Name = "Cód. Barras")]
        [Required]
        public string CodigoDeBarras { get; set; }

        [Display(Name = "Referência")]
        [Required]
        public string Referencia { get; set; }

        [Display(Name = "Qtde.")]
        [Required]
        public int? Quantidade { get; set; }

        [Display(Name = "Unidade")]
        [Required]
        public string Unidade { get; set; }

        [Display(Name = "Fabricante")]
        [Required]
        public string Fabricante { get; set; }

        [Display(Name = "Tipo")]
        public string Tipo { get; set; }
    }
}
using DartDigital.Library.Web.ModelValidation;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.ExampleCtx
{
    public class TesteValidacaoViewModel
    {
        public string Message { get; set; }

        [MvcEmailValidation]
        [Required]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [MvcUrlValidation]
        [Required]
        [Display(Name = "URL")]
        public string Url { get; set; }

        [MvcIPAddressValidation]
        [Required]
        [Display(Name = "IP")]
        public string IP { get; set; }

        [MvcCpfValidation]
        [Required]
        [Display(Name = "CPF")]
        public string Cpf { get; set; }

        [MvcCnpjValidation]
        [Required]
        [Display(Name = "CNPJ")]
        public string Cnpj { get; set; }

        [MvcCpfOrCnpjValidation]
        [Required]
        [Display(Name = "CPF/CNPJ")]
        public string CpfOrCnpj { get; set; }

        [MvcCepValidation]
        [Required]
        [Display(Name = "CEP")]
        public string Cep { get; set; }

        [MvcAlphaOnlyValidation]
        [Required]
        [Display(Name = "Alfabeto sem Números")]
        public string AlphaOnly { get; set; }

        [MvcBrazilPhoneValidation]
        [Required]
        [Display(Name = "Telefone Brasil")]
        public string BrazilPhone { get; set; }

        public bool CheckTest { get; set; }

        [Display(Name = "Requerido opcionalmente")]
        [RequiredIf(nameof(IP), "192")]
        public string RequiredIf { get; set; }
    }
}
using FWLog.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BOEmpresaCtx
{
    public class EmpresaConfigEditViewModel
    {
        [Required]
        public long IdEmpresa { get; set; }

        public long IdEmpresaConfig { get; set; }

        [Required]
        [Display(Name = "Empresa Garantia")]
        public long IdEmpresaGarantia { get; set; }

        [Required]
        [Display(Name = "Empresa Matriz")]
        public long IdEmpresaMatriz { get; set; }

        [Required]
        [Display(Name = "Tipo da Empresa")]
        public EmpresaTipoEnum IdEmpresaTipo { get; set; }

        [Required]
        [Display(Name = "Tipo da Conferência")]
        public EmpresaTipoEnum IdTipoConferencia { get; set; }

        public string RazaoSocialEmpresaMatriz { get; set; }

        public string RazaoSocialEmpresaGarantia { get; set; }

        [Display(Name = "Empresa Faz Garantia?")]
        public bool EmpresaFazGarantia { get; set; }

        public EmpresaDetailsViewModel Empresa { get; set; }
    }

    public class EmpresaDetailsViewModel
    {
        public long IdEmpresa { get; set; }

        [Display(Name = "Razão Social")]
        public string RazaoSocial { get; set; }

        [Display(Name = "Sigla")]
        public string Sigla { get; set; }

        [Display(Name = "Nome Fantasia")]
        public string NomeFantasia { get; set; }

        [Display(Name = "CNPJ")]
        public string CNPJ { get; set; }

        [Display(Name = "CEP")]
        public string CEP { get; set; }

        [Display(Name = "Endereço")]
        public string Endereco { get; set; }

        [Display(Name = "Número")]
        public string Numero { get; set; }

        [Display(Name = "Complemento")]
        public string Complemento { get; set; }

        [Display(Name = "Bairro")]
        public string Bairro { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; }

        [Display(Name = "Cidade")]
        public string Cidade { get; set; }

        [Display(Name = "Telefone")]
        public string Telefone { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; }

        [Display(Name = "Código de Integracao")]
        public int CodigoIntegracao { get; set; }
    }
}

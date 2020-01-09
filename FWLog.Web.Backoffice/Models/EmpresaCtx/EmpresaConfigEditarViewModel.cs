using FWLog.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.EmpresaCtx
{
    public class EmpresaConfigEditarViewModel
    {
        [Required]
        public long IdEmpresa { get; set; }
        [Required]
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
        [Display(Name = "Tipo da Conferência")]
        public TipoConferenciaEnum? IdTipoConferencia { get; set; }
        [Display(Name = "Empresa Faz Garantia?")]
        public bool EmpresaFazGarantia { get; set; }
        [Display(Name = "CNPJ Conferência Automática")]
        public string CNPJConferenciaAutomatica { get; set; }

        public EmpresaDetalhesViewModel Empresa { get; set; }

        public string RazaoSocialEmpresaMatriz { get; set; }
        public string RazaoSocialEmpresaGarantia { get; set; }

        public SelectList TiposEmpresa { get; set; }
        public SelectList TiposConferencia { get; set; }
    }

    public class EmpresaDetalhesViewModel
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

        [Display(Name = "Ativo?")]
        public bool Ativo { get; set; }

        [Display(Name = "Código de Integracao")]
        public int CodigoIntegracao { get; set; }
    }
}

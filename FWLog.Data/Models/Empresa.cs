using FWLog.Data.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    [Table("Company")]
    public class Empresa
    {
        [Key]
        [Required]
        [Column("CompanyId")]
        public long IdEmpresa { get; set; }

        [Required]
        [StringLength(40)]
        [Column("CompanyName")]
        public string RazaoSocial { get; set; }

        [Required]
        [StringLength(3)]
        [Column("Initials")]
        public string Sigla { get; set; }

        [Column("TradingName")]
        [StringLength(40)]
        public string NomeFantasia { get; set; }

        [Column("CNPJ")]
        [StringLength(14)]
        public string CNPJ { get; set; }

        [Column("AddressZipCode")]
        public int? CEP { get; set; }

        [Column("Address")]
        [StringLength(76)]
        public string Endereco { get; set; }

        [Column("AddressNumber")]
        [StringLength(6)]
        public string Numero { get; set; }

        [Column("AddressComplement")]
        [StringLength(10)]
        public string Complemento { get; set; }

        [Column("AddressNeighborhood")]
        [StringLength(50)]
        public string Bairro { get; set; }

        [Column("AddressState")]
        [StringLength(40)]
        public string Estado { get; set; }

        [Column("AddressCity")]
        [StringLength(50)]
        public string Cidade { get; set; }

        [Column("PhoneNumber")]
        [StringLength(15)]
        public string Telefone { get; set; }

        [Column("Disabled")]
        public bool Ativo { get; set; }

        [Required]
        public int CodigoIntegracao { get; set; }

        [Required]
        public long IdEmpresaTipo { get; set; }

        [Required]
        public bool EmpresaFazGarantia { get; set; }

        public long? IdEmpresaMatriz { get; set; }
                
        public long? IdEmpresaGarantia { get; set; }

        [ForeignKey(nameof(IdEmpresaMatriz))]
        public virtual Empresa EmpresaMatriz { get; set; }

        [ForeignKey(nameof(IdEmpresaGarantia))]
        public virtual Empresa EmpresaGarantia { get; set; }

        [InverseProperty("EmpresaMatriz")]
        public virtual ICollection<Empresa> EmpresasFiliais { get; set; }

        [InverseProperty("EmpresaGarantia")]
        public virtual ICollection<Empresa> EmpresasGarantia { get; set; }

        [ForeignKey(nameof(IdEmpresaTipo))]
        public virtual EmpresaTipo EmpresaTipo { get; set; }
    }
}

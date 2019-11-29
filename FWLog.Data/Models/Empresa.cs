using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class Empresa
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdEmpresa { get; set; }

        [Required]
        [StringLength(40)]
        [Index]
        public string RazaoSocial { get; set; }

        [Required]
        [StringLength(3)]
        public string Sigla { get; set; }

        [StringLength(40)]
        [Index]
        public string NomeFantasia { get; set; }

        [StringLength(14)]
        [Index]
        public string CNPJ { get; set; }

        public int? CEP { get; set; }

        [StringLength(76)]
        public string Endereco { get; set; }

        [StringLength(6)]
        public string Numero { get; set; }

        [StringLength(10)]
        public string Complemento { get; set; }

        [StringLength(50)]
        public string Bairro { get; set; }

        [StringLength(40)]
        public string Estado { get; set; }

        [StringLength(50)]
        public string Cidade { get; set; }

        [StringLength(15)]
        public string Telefone { get; set; }

        public bool Ativo { get; set; }

        [Required]
        [Index]
        public int CodigoIntegracao { get; set; }

    }
}

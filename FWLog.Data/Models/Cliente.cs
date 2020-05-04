using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class Cliente
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdCliente { get; set; }

        [Required]
        [Index]
        [StringLength(180)]
        public string NomeFantasia { get; set; }

        [Required]
        [Index]
        [StringLength(75)]
        public string RazaoSocial { get; set; }

        [Required]
        [Index]
        [StringLength(14)]
        public string CNPJCPF { get; set; }

        [Required]
        [Index]
        public long CodigoIntegracao { get; set; }

        [StringLength(14)]
        public string Classificacao { get; set; }

        [Required]
        public bool Ativo { get; set; }

        [Index]
        public long? IdRepresentanteExterno { get; set; }

        [Index]
        public long? IdRepresentanteInterno { get; set; }

        [Required]
        [StringLength(8)]
        public string CEP { get; set; }

        [Required]
        [StringLength(76)]
        public string Endereco { get; set; }

        [Required]
        [StringLength(6)]
        public string Numero { get; set; }

        [Required]
        [StringLength(13)]
        public string Telefone { get; set; }

        [StringLength(2)]
        [Required]
        public string UF { get; set; }

        [StringLength(50)]
        [Required]
        public string Cidade { get; set; }

        [ForeignKey(nameof(IdRepresentanteInterno))]
        public virtual Representante RepresentanteInterno { get; set; }

        [ForeignKey(nameof(IdRepresentanteExterno))]
        public virtual Representante RepresentanteExterno { get; set; }
    }
}
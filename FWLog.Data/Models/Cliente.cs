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

        [Required]
        [StringLength(14)]
        public string Classificacao { get; set; }

        [Required]
        public bool Ativo { get; set; }

        [Index]
        public long? IdRepresentante { get; set; }

        // Descomentar quando finalizar a integração de Cliente
        //[Index]
        //public long? IdVendedor { get; set; }

        [ForeignKey(nameof(IdRepresentante))]
        public virtual Representante Representante { get; set; }

        // Descomentar quando finalizar a integração de Cliente
        //[Index]
        //[ForeignKey(nameof(IdVendedor))]
        //public virtual Representante Vendedor { get; set; }
    }
}

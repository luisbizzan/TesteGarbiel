using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class Fornecedor
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdFornecedor { get; set; }

        [Required]
        [Index]
        public long CodigoIntegracao { get; set; }

        [Required]
        [Index]
        [StringLength(180)]
        public string NomeFantasia { get; set; }

        [Index]
        [StringLength(75)]
        public string RazaoSocial { get; set; }

        [Index]
        [StringLength(14)]
        public string CNPJ { get; set; }

        [Required]
        public bool   Ativo          { get; set; }

        [StringLength(8)]
        public string CEP { get; set; }

        [StringLength(76)]
        public string Endereco { get; set; }

        [StringLength(6)]
        public string Numero { get; set; }

        [StringLength(30)]
        public string Complemento { get; set; }

        [StringLength(50)]
        public string Bairro { get; set; }

        [StringLength(40)]
        public string Estado { get; set; }

        [StringLength(50)]
        public string Cidade { get; set; }

        [StringLength(15)]
        public string Telefone { get; set; }


        //TODO criar configuração de não paga garantia e transportadora padrão

    }
}

using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models
{
    public class Fornecedor
    {
        [Key]
        public long IdFornecedor { get; set; }
        public long CodigoIntegracao { get; set; }
        public string NomeFantasia { get; set; }
        public string RazaoSocial { get; set; }
        public string CNPJ { get; set; }
    }
}

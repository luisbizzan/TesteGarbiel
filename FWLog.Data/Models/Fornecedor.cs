using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models
{
    public class Fornecedor
    {
        [Key]
        public long IdFornecedor { get; set; }
        public string Codigo { get; set; }
        public string NomeFantasia { get; set; }
        public string RazaoSocial { get; set; }
        public string CNPJ { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models
{
    public class Transportadora
    {
        [Key]
        public int IdTransportadora { get; set; }
        public string NomeFantasia { get; set; }
        public string RazaoSocial { get; set; }
        public string CNPJ { get; set; }
    }
}

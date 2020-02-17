using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum IntegracaoEntidadeEnum
    {
        Token            = 1,
        Empresa          = 2,
        Produto          = 3,
        FreteTipo        = 4,
        UnidadeMedida    = 5,
        Transportadora   = 6,
        Fornecedor       = 7,
        NotaFiscalCompra = 8
    }

    public class IntegracaoEntidade
    {
        [Key]
        [Index(IsUnique = true)]
        [Required]
        public IntegracaoEntidadeEnum IdIntegracaoEntidade { get; set; }

        [StringLength(50)]
        [Index(IsUnique = true)]
        [Required]
        public string Descricao { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum IntegracaoTipoEnum
    {
        Login                          = 1,
        BuscarEmpresa                  = 2,
        BuscarFreteTipo                = 3,
        BuscarProduto                  = 4,
        BuscarUnidadeMedida            = 5,
        BuscarTransportadora           = 6,
        BuscarFornecedor               = 7,
        BuscarNotaFiscalCompra         = 8,
        AtualizarStatusIntegracao      = 9,
        AlterarStatusNotaFiscalCompra  = 10,
        ConfirmarNotaFiscalCompra      = 11,
        IncluirNotaFiscalDevolucao     = 12
    }

    public class IntegracaoTipo
    {
        [Key]
        [Index(IsUnique = true)]
        [Required]
        public IntegracaoTipoEnum IdIntegracaoTipo { get; set; }

        [StringLength(100)]
        [Index(IsUnique = true)]
        [Required]
        public string Descricao { get; set; }
    }
}

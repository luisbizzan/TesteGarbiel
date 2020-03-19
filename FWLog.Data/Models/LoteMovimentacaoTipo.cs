using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models
{
    public enum LoteMovimentacaoTipoEnum
    {
        Entrada = 1,
        Saida = 2,
        Ajuste = 3,
        Abastecimento = 4
    }

    public class LoteMovimentacaoTipo
    {
        [Key]
        [Required]
        public LoteMovimentacaoTipoEnum IdLoteMovimentacaoTipo { get; set; }

        [Required]
        public string Descricao { get; set; }
    }
}

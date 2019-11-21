using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class Produto
    {
        [Key]
        public long IdProduto { get; set; }
        public string Descricao { get; set; }
        public string Referencia { get; set; }
        public long IdUnidadeMedida { get; set; }
        public decimal PesoBruto { get; set; }

        [ForeignKey(nameof(IdUnidadeMedida))]
        public UnidadeMedida UnidadeMedida { get; set; }
    }
}

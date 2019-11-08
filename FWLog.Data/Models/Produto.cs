namespace FWLog.Data.Models
{
    public class Produto
    {
        public int IdProduto { get; set; }
        public string Descricao { get; set; }
        public string Referencia { get; set; }
        public int IdUnidadeMedida { get; set; }
        public decimal Peso { get; set; }

        public virtual UnidadeMedida UnidadeMedida { get; set; }
    }
}

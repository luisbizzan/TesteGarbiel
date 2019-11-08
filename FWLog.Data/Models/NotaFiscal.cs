namespace FWLog.Data.Models
{
    public class NotaFiscal
    {
        public int IdNotaFiscal { get; set; }
        public int Numero { get; set; }
        public int Serie { get; set; }
        public string DANFE { get; set; }
        public int IdFornecedor { get; set; }
        public decimal ValorTotal { get; set; }
        public int IdTransportadora { get; set; }
        public int IdFreteTipo { get; set; }
        public decimal ValorFrete { get; set; }
        public int NumeroConhecimento { get; set; }
        public decimal PesoBruto { get; set; }
        public decimal PesoLiquido { get; set; }
        public string Especie { get; set; }
        public int Quantidade { get; set; }

        public virtual Fornecedor Fornecedor { get; set; }
        public virtual Transportadora Transportadora { get; set; }
        public virtual FreteTipo FreteTipo { get; set; }
    }
}

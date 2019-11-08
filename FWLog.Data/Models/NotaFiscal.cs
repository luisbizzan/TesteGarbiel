using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class NotaFiscal
    {
        [Key]
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
        public string Especie { get; set; }
        public int Quantidade { get; set; }
        public string Status { get; set; }
        public string Chave { get; set; }

        [ForeignKey(nameof(IdFornecedor))]
        public Fornecedor Fornecedor { get; set; }

        [ForeignKey(nameof(IdTransportadora))]
        public Transportadora Transportadora { get; set; }

        [ForeignKey(nameof(IdFreteTipo))]
        public FreteTipo FreteTipo { get; set; }
    }
}

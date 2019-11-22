using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class NotaFiscal
    {
        [Key]
        public long IdNotaFiscal { get; set; }
        public int Numero { get; set; }
        public int? Serie { get; set; }
        public string DANFE { get; set; }
        public long IdFornecedor { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime PrazoEntregaFornecedor { get; set; } //TODO Aguardando resposta do Geovane para verificar se este campo deve ficar nesta classe
        public decimal ValorTotal { get; set; }
        public long IdTransportadora { get; set; }
        public long IdFreteTipo { get; set; }
        public decimal ValorFrete { get; set; }
        public long? NumeroConhecimento { get; set; }
        public decimal? PesoBruto { get; set; }        
        public string Especie { get; set; }
        public int Quantidade { get; set; }
        public string StatusIntegracao { get; set; }
        public long IdNotaFiscalStatus { get; set; }
        public string Chave { get; set; }
        public long CodigoIntegracao { get; set; }
        public long CompanyId { get; set; }

        [ForeignKey(nameof(IdFornecedor))]
        public virtual Fornecedor Fornecedor { get; set; }

        [ForeignKey(nameof(IdTransportadora))]
        public virtual Transportadora Transportadora { get; set; }

        [ForeignKey(nameof(IdFreteTipo))]
        public virtual FreteTipo FreteTipo { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey(nameof(IdNotaFiscalStatus))]
        public virtual NotaFiscalStatus NotaFiscalStatus { get; set; }
    }
}

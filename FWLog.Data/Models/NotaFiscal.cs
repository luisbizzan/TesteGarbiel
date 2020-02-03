using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class NotaFiscal
    {
        public NotaFiscal()
        {
            NotaFiscalItens = new HashSet<NotaFiscalItem>();
        }

        [Key]
        [Required]
        public long IdNotaFiscal { get; set; }

        [Index]
        [Required]
        public long IdEmpresa { get; set; }

        [Index]
        public long IdFornecedor { get; set; }

        [Index]
        [Required]
        public int Numero { get; set; }

        [Index]
        public long? IdFreteTipo { get; set; }

        [Index]
        [Required]
        public NotaFiscalStatusEnum IdNotaFiscalStatus { get; set; }

        [Index]
        [Required]
        public long CodigoIntegracao { get; set; }

        [Index]
        [Required]
        public long IdTransportadora { get; set; }

        public long? NumeroConhecimento { get; set; }

        [Required]
        public int Quantidade { get; set; }

        [Index]
        [StringLength(3)]
        public string Serie { get; set; }

        [Index]
        public string ChaveAcesso { get; set; }

        [Required]
        [StringLength(1)]
        public string StatusIntegracao { get; set; }

        public string Especie { get; set; }

        [StringLength(20)]
        public string NumeroFicticioNF { get; set; }

        public DateTime DataEmissao { get; set; }

        public DateTime PrazoEntregaFornecedor { get; set; }

        [Required]
        public decimal ValorTotal { get; set; }

        [Required]
        public decimal ValorFrete { get; set; }

        public decimal? PesoBruto { get; set; }

        [Required]
        public int CodigoIntegracaoVendedor { get; set; }

        public long? CodigoIntegracaoNFDevolucao { get; set; }

        [Required]
        public NotaFiscalTipoEnum IdNotaFiscalTipo { get; set; }

        public bool NFDevolucaoConfirmada { get; set; }

        [ForeignKey(nameof(IdFornecedor))]
        public virtual Fornecedor Fornecedor { get; set; }

        [ForeignKey(nameof(IdTransportadora))]
        public virtual Transportadora Transportadora { get; set; }

        [ForeignKey(nameof(IdFreteTipo))]
        public virtual FreteTipo FreteTipo { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey(nameof(IdNotaFiscalStatus))]
        public virtual NotaFiscalStatus NotaFiscalStatus { get; set; }

        public virtual ICollection<NotaFiscalItem> NotaFiscalItens { get; set; }
    }
}

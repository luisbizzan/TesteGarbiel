﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class NotaFiscalItem
    {
        [Key]
        [Required]
        public long IdNotaFiscalItem { get; set; }

        [Required]
        [Index]
        public long IdNotaFiscal { get; set; }

        [Required]
        [Index]
        public long IdProduto { get; set; }

        [Required]
        [Index]
        public long IdUnidadeMedida { get; set; }

        [Required]
        public int Quantidade { get; set; }

        [Required]
        public int QuantidadeDevolucao { get; set; }

        [Required]
        public decimal ValorUnitario { get; set; }

        [Required]
        public decimal ValorTotal { get; set; }

        [Required]
        public int Sequencia { get; set; }

        [Required]
        [Index]
        public long CodigoNotaFiscal { get; set; }
        
        public int? CodigoIntegracaoNFOrigem { get; set; }

        public int? SequenciaNFOrigem { get; set; }

        public long? CFOP { get; set; }

        public long? CodigoBarras { get; set; }


        [ForeignKey(nameof(IdNotaFiscal))]
        public virtual NotaFiscal NotaFiscal { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public virtual Produto Produto { get; set; }

        [ForeignKey(nameof(IdUnidadeMedida))]
        public virtual UnidadeMedida UnidadeMedida { get; set; }
    }
}

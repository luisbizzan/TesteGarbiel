﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class LoteMovimentacao
    {
        [Key]
        [Required]
        public long IdLoteMovimentacao { get; set; }
        [Index]
        [Required]
        public long IdEmpresa { get; set; }
        [Index]
        [Required]
        public long IdLote { get; set; }
        [Index]
        public long? IdProduto { get; set; }
        [Index]
        [Required]
        public long IdEnderecoArmazenagem { get; set; }
        [Required]
        public string IdUsuarioMovimentacao { get; set; }
        [Required]
        public LoteMovimentacaoTipoEnum IdLoteMovimentacaoTipo { get; set; }
        [Required]
        public int Quantidade { get; set; }
        [Required]
        public DateTime DataHora { get; set; }
        public int? NroVolume { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey(nameof(IdLote))]
        public virtual Lote Lote { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public virtual Produto Produto { get; set; }

        [ForeignKey(nameof(IdEnderecoArmazenagem))]
        public virtual EnderecoArmazenagem EnderecoArmazenagem { get; set; }

        [ForeignKey(nameof(IdUsuarioMovimentacao))]
        public virtual AspNetUsers Usuario { get; set; }

        [ForeignKey(nameof(IdLoteMovimentacaoTipo))]
        public virtual LoteMovimentacaoTipo LoteMovimentacaoTipo { get; set; }
    }
}
